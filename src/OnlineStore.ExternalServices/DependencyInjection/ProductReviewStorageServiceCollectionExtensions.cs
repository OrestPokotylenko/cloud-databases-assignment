using Azure.Data.Tables;

using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

using OnlineStore.Application.ProductReviews.Abstractions;
using OnlineStore.ExternalServices.Options;
using OnlineStore.ExternalServices.TableStorage;

namespace OnlineStore.ExternalServices.DependencyInjection;

public static class ProductReviewStorageServiceCollectionExtensions
{
    public static IServiceCollection AddProductReviewStorageServices(
        this IServiceCollection services)
    {
        services.AddOptions<ProductReviewStorageOptions>()
            .BindConfiguration(ProductReviewStorageOptions.SectionName)
            .Validate(
                options => !string.IsNullOrWhiteSpace(options.ConnectionString),
                $"{ProductReviewStorageOptions.SectionName}:ConnectionString is required.")
            .Validate(
                options => IsValidTableName(options.TableName),
                $"{ProductReviewStorageOptions.SectionName}:TableName must contain 3 to 63 alphanumeric characters and start with a letter.")
            .ValidateOnStart();

        services.AddSingleton(provider =>
        {
            var options = provider
                .GetRequiredService<IOptions<ProductReviewStorageOptions>>()
                .Value;

            return new TableClient(
                options.ConnectionString,
                options.TableName);
        });
        services.AddSingleton<IProductReviewRepository, AzureTableProductReviewRepository>();

        return services;
    }

    private static bool IsValidTableName(string? tableName) =>
        tableName is { Length: >= 3 and <= 63 } &&
        char.IsLetter(tableName[0]) &&
        tableName.All(char.IsLetterOrDigit);
}
