using Azure.Storage.Blobs;

using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

using OnlineStore.Application.Products.Images;
using OnlineStore.ExternalServices.BlobStorage;
using OnlineStore.ExternalServices.Options;

namespace OnlineStore.ExternalServices.DependencyInjection;

public static class ProductImageStorageServiceCollectionExtensions
{
    public static IServiceCollection AddProductImageStorageServices(
        this IServiceCollection services)
    {
        services.AddOptions<ProductImageStorageOptions>()
            .BindConfiguration(ProductImageStorageOptions.SectionName)
            .Validate(
                options => !string.IsNullOrWhiteSpace(options.ConnectionString),
                $"{ProductImageStorageOptions.SectionName}:ConnectionString is required.")
            .Validate(
                options => !string.IsNullOrWhiteSpace(options.ContainerName),
                $"{ProductImageStorageOptions.SectionName}:ContainerName is required.")
            .Validate(
                options => options.UploadUrlLifetimeMinutes is > 0 and <= 60,
                $"{ProductImageStorageOptions.SectionName}:UploadUrlLifetimeMinutes must be between 1 and 60.")
            .ValidateOnStart();

        services.AddOptions<ProductImageValidationOptions>()
            .BindConfiguration(ProductImageValidationOptions.SectionName)
            .Validate(
                options => options.MaximumSizeBytes > 0,
                $"{ProductImageValidationOptions.SectionName}:MaximumSizeBytes must be positive.")
            .Validate(
                options => options.SupportedContentTypes is { Length: > 0 },
                $"{ProductImageValidationOptions.SectionName}:SupportedContentTypes must not be empty.")
            .ValidateOnStart();

        services.AddSingleton(provider =>
        {
            var options = provider
                .GetRequiredService<IOptions<ProductImageStorageOptions>>()
                .Value;

            return new BlobContainerClient(
                options.ConnectionString,
                options.ContainerName);
        });
        services.AddSingleton<IProductImageStorage, AzureBlobProductImageStorage>();

        return services;
    }
}
