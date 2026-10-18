using Microsoft.Extensions.DependencyInjection;

using OnlineStore.Application.Customers.Abstractions;
using OnlineStore.Persistence.Customers;

namespace OnlineStore.Persistence.DependencyInjection;

internal static class CustomerPersistenceServiceCollectionExtensions
{
    public static IServiceCollection AddCustomerPersistence(
        this IServiceCollection services)
    {
        services.AddScoped<CustomerRepository>();
        services.AddScoped<ICustomerCommandRepository>(provider =>
            provider.GetRequiredService<CustomerRepository>());
        services.AddScoped<ICustomerQueryRepository>(provider =>
            provider.GetRequiredService<CustomerRepository>());

        return services;
    }
}
