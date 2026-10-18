using OnlineStore.Application.Abstractions.Messaging;

namespace OnlineStore.Application.Orders.Messages;

public record CreateOrderMessage(
    Guid MessageId,
    Guid CorrelationId,
    DateTimeOffset SubmittedAt,
    Guid OrderPublicId,
    Guid CustomerPublicId,
    DeliveryAddressMessage DeliveryAddress,
    IReadOnlyList<OrderItemMessage> Items)
    : IQueuedCommand;
