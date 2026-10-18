using OnlineStore.Application.Abstractions.CQRS;

namespace OnlineStore.Application.Abstractions.Messaging;

public interface IQueuedCommand : ICommand<QueuedCommandResult>
{
    Guid MessageId { get; }
}
