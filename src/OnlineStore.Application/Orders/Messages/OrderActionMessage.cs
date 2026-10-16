using OnlineStore.Application.Abstractions.Messaging;

namespace OnlineStore.Application.Orders.Messages;

public record OrderActionMessage(
    Guid MessageId,
    Guid CorrelationId,
    DateTimeOffset RequestedAt,
    Guid OrderPublicId,
    OrderAction Action)
    : IQueuedCommand;
