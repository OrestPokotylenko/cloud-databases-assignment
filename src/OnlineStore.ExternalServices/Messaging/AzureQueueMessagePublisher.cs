using System.Text.Json;

using Azure.Storage.Queues;
using OnlineStore.Application.Abstractions.Messaging;

namespace OnlineStore.ExternalServices.Messaging;

internal class AzureQueueMessagePublisher<TMessage>(QueueClient queueClient)
    : IMessagePublisher<TMessage>
{
    private readonly SemaphoreSlim initializationLock = new(1, 1);
    private volatile bool initialized;

    private readonly JsonSerializerOptions jsonOptions =
        QueueMessageJsonOptions.Create();

    public async Task PublishAsync(
        TMessage message,
        CancellationToken ct)
    {
        ArgumentNullException.ThrowIfNull(message);

        await EnsureQueueExistsAsync(ct);
        var content = JsonSerializer.Serialize(message, jsonOptions);
        await queueClient.SendMessageAsync(content, ct);
    }

    private async Task EnsureQueueExistsAsync(CancellationToken ct)
    {
        if (initialized)
        {
            return;
        }

        await initializationLock.WaitAsync(ct);
        try
        {
            if (!initialized)
            {
                await queueClient.CreateIfNotExistsAsync(cancellationToken: ct);
                initialized = true;
            }
        }
        finally
        {
            initializationLock.Release();
        }
    }
}
