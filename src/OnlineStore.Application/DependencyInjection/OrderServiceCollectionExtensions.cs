using Microsoft.Extensions.DependencyInjection;

using OnlineStore.Application.Abstractions.CQRS;
using OnlineStore.Application.Abstractions.Messaging;
using OnlineStore.Application.Orders.Actions;
using OnlineStore.Application.Orders.CancelOrder;
using OnlineStore.Application.Orders.CreateOrder;
using OnlineStore.Application.Orders.GetOrderById;
using OnlineStore.Application.Orders.GetOrders;
using OnlineStore.Application.Orders.Messages;
using OnlineStore.Application.Orders.ProcessCreateOrder;
using OnlineStore.Application.Orders.ProcessOrder;
using OnlineStore.Application.Orders.ProcessOrderAction;
using OnlineStore.Application.Orders.ShipOrder;

namespace OnlineStore.Application.DependencyInjection;

internal static class OrderServiceCollectionExtensions
{
    public static IServiceCollection AddOrderServices(
        this IServiceCollection services)
    {
        services.AddScoped<IHandler<CreateOrderCommand, CreateOrderResult>, CreateOrderHandler>();
        services.AddScoped<IHandler<ProcessOrderCommand, OrderActionResult>, OrderActionHandler>();
        services.AddScoped<IHandler<ShipOrderCommand, OrderActionResult>, OrderActionHandler>();
        services.AddScoped<IHandler<CancelOrderCommand, OrderActionResult>, OrderActionHandler>();
        services.AddScoped<IHandler<GetOrderByIdQuery, GetOrderByIdResult?>, GetOrderByIdHandler>();
        services.AddScoped<IHandler<GetOrdersQuery, GetOrdersResult>, GetOrdersHandler>();
        services.AddScoped<IHandler<CreateOrderMessage, QueuedCommandResult>, ProcessCreateOrderHandler>();
        services.AddScoped<IHandler<OrderActionMessage, QueuedCommandResult>, ProcessOrderActionHandler>();

        return services;
    }
}
