using OnlineStore.Application.Abstractions.Messaging;

namespace OnlineStore.Application.UnitTests.Orders;

internal class MessagePublisherStub<TMessage> : IMessagePublisher<TMessage>
{
    public TMessage? PublishedMessage { get; private set; }

    public Task PublishAsync(
        TMessage message,
        CancellationToken ct)
    {
        PublishedMessage = message;
        return Task.CompletedTask;
    }
}
