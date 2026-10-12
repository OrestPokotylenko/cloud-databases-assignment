using Microsoft.Extensions.DependencyInjection;

using OnlineStore.Application.Products.Abstractions;
using OnlineStore.Persistence.Products;

namespace OnlineStore.Persistence.DependencyInjection;

internal static class ProductPersistenceServiceCollectionExtensions
{
    public static IServiceCollection AddProductPersistence(
        this IServiceCollection services)
    {
        services.AddScoped<ProductRepository>();
        services.AddScoped<IProductCommandRepository>(provider =>
            provider.GetRequiredService<ProductRepository>());
        services.AddScoped<IProductQueryRepository>(provider =>
            provider.GetRequiredService<ProductRepository>());

        return services;
    }
}
