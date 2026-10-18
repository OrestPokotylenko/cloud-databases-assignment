namespace OnlineStore.Application.Abstractions.Messaging;

public interface IMessagePublisher<in TMessage>
{
    Task PublishAsync(
        TMessage message,
        CancellationToken ct);
}
