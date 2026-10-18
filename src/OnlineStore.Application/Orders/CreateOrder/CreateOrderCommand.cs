using OnlineStore.Application.Abstractions.CQRS;
using OnlineStore.Application.Orders.Messages;

namespace OnlineStore.Application.Orders.CreateOrder;

public record CreateOrderCommand(
    Guid CustomerPublicId,
    DeliveryAddressMessage DeliveryAddress,
    IReadOnlyList<OrderItemMessage> Items)
    : ICommand<CreateOrderResult?>;
