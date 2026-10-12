using Microsoft.Extensions.DependencyInjection;

using OnlineStore.Application.Abstractions.CQRS;
using OnlineStore.Application.Abstractions.Messaging;
using OnlineStore.Application.Orders.CreateOrder;
using OnlineStore.Application.Orders.GetOrderById;
using OnlineStore.Application.Orders.GetOrders;
using OnlineStore.Application.Orders.Messages;
using OnlineStore.Application.Orders.ProcessCreateOrder;
using OnlineStore.Application.Orders.ProcessUpdateOrderStatus;
using OnlineStore.Application.Orders.UpdateOrderStatus;

namespace OnlineStore.Application.DependencyInjection;

internal static class OrderServiceCollectionExtensions
{
    public static IServiceCollection AddOrderServices(
        this IServiceCollection services)
    {
        services.AddScoped<IHandler<CreateOrderCommand, CreateOrderResult>, CreateOrderHandler>();
        services.AddScoped<IHandler<UpdateOrderStatusCommand, UpdateOrderStatusResult>, UpdateOrderStatusHandler>();
        services.AddScoped<IHandler<GetOrderByIdQuery, GetOrderByIdResult?>, GetOrderByIdHandler>();
        services.AddScoped<IHandler<GetOrdersQuery, GetOrdersResult>, GetOrdersHandler>();
        services.AddScoped<IHandler<CreateOrderMessage, QueuedCommandResult>, ProcessCreateOrderHandler>();
        services.AddScoped<IHandler<UpdateOrderStatusMessage, QueuedCommandResult>, ProcessUpdateOrderStatusHandler>();

        return services;
    }
}
