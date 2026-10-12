using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

using OnlineStore.Persistence.Options;

namespace OnlineStore.Persistence.DependencyInjection;

public static class PersistenceServiceCollectionExtensions
{
    public static IServiceCollection AddPersistenceServices(
        this IServiceCollection services)
    {
        services.AddOptions<DatabaseOptions>()
            .BindConfiguration(DatabaseOptions.SectionName)
            .Validate(
                options => !string.IsNullOrWhiteSpace(options.ConnectionString),
                $"{DatabaseOptions.SectionName}:ConnectionString is required.")
            .ValidateOnStart();

        services.AddDbContext<OnlineStoreDbContext>((provider, options) =>
        {
            var database = provider
                .GetRequiredService<IOptions<DatabaseOptions>>()
                .Value;

            options.UseSqlServer(database.ConnectionString);
        });

        services.AddOrderPersistence();
        services.AddProductPersistence();

        return services;
    }
}
