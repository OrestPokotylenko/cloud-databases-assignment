using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

using OnlineStore.Application.Products.Abstractions;
using OnlineStore.Persistence.Products;

namespace OnlineStore.Persistence;

public static class DependencyInjection
{
    public static IServiceCollection AddPersistence(
        this IServiceCollection services,
        string connectionString)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(connectionString);

        services.AddDbContext<OnlineStoreDbContext>(options =>
            options.UseSqlServer(connectionString));

        services.AddScoped<ProductRepository>();

        services.AddScoped<IProductCommandRepository>(provider =>
            provider.GetRequiredService<ProductRepository>());

        services.AddScoped<IProductQueryRepository>(provider =>
            provider.GetRequiredService<ProductRepository>());

        return services;
    }
}
