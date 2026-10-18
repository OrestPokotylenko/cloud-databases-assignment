using Microsoft.Extensions.DependencyInjection;

using OnlineStore.Application.Products.Abstractions;
using OnlineStore.Application.Products.Images;
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

        services.AddScoped<ProductImageRepository>();
        services.AddScoped<IProductImageCommandRepository>(provider =>
            provider.GetRequiredService<ProductImageRepository>());
        services.AddScoped<IProductImageQueryRepository>(provider =>
            provider.GetRequiredService<ProductImageRepository>());

        return services;
    }
}
