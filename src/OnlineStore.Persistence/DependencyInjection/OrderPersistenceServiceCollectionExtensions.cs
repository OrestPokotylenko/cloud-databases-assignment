using Microsoft.Extensions.DependencyInjection;

using OnlineStore.Application.Orders.Abstractions;
using OnlineStore.Persistence.Orders;

namespace OnlineStore.Persistence.DependencyInjection;

internal static class OrderPersistenceServiceCollectionExtensions
{
    public static IServiceCollection AddOrderPersistence(
        this IServiceCollection services)
    {
        services.AddScoped<OrderRepository>();
        services.AddScoped<IOrderCommandRepository>(provider =>
            provider.GetRequiredService<OrderRepository>());
        services.AddScoped<IOrderQueryRepository>(provider =>
            provider.GetRequiredService<OrderRepository>());

        return services;
    }
}
