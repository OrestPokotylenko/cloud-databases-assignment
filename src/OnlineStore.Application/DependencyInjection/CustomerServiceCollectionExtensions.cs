using Microsoft.Extensions.DependencyInjection;

using OnlineStore.Application.Abstractions.CQRS;
using OnlineStore.Application.Customers;
using OnlineStore.Application.Customers.CreateCustomer;
using OnlineStore.Application.Customers.GetCustomer;
using OnlineStore.Application.Customers.UpdateCustomer;

namespace OnlineStore.Application.DependencyInjection;

internal static class CustomerServiceCollectionExtensions
{
    public static IServiceCollection AddCustomerServices(
        this IServiceCollection services)
    {
        services.AddScoped<IHandler<CreateCustomerCommand, CreateCustomerResult>, CreateCustomerHandler>();
        services.AddScoped<IHandler<GetCustomerQuery, CustomerResult?>, GetCustomerHandler>();
        services.AddScoped<IHandler<UpdateCustomerCommand, UpdateCustomerResult>, UpdateCustomerHandler>();

        return services;
    }
}
