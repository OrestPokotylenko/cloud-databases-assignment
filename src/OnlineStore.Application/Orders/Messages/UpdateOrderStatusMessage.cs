using OnlineStore.Application.Abstractions.Messaging;
using OnlineStore.Domain.Orders;

namespace OnlineStore.Application.Orders.Messages;

public record UpdateOrderStatusMessage(
    Guid MessageId,
    Guid CorrelationId,
    DateTimeOffset RequestedAt,
    Guid OrderPublicId,
    OrderStatus Status)
    : IQueuedCommand;
