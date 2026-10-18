using System.Text.Json;

using Microsoft.Extensions.Logging;

using OnlineStore.Application.Abstractions.CQRS;
using OnlineStore.Application.Abstractions.Messaging;

namespace OnlineStore.Functions.Messaging;

public class QueueCommandProcessor(
    IRequestSender sender,
    ILogger<QueueCommandProcessor> logger)
{
    private readonly JsonSerializerOptions jsonOptions =
        QueueMessageJsonOptions.Create();

    public async Task ProcessAsync<TCommand>(
        string content,
        CancellationToken ct)
        where TCommand : IQueuedCommand
    {
        var command = JsonSerializer.Deserialize<TCommand>(content, jsonOptions)
            ?? throw new InvalidOperationException("The queue message is empty or invalid.");

        using var scope = logger.BeginScope(
            new Dictionary<string, object>
            {
                ["MessageId"] = command.MessageId,
                ["MessageType"] = typeof(TCommand).Name
            });

        logger.LogInformation("Processing queued command.");
        await sender.SendAsync(command, ct);
        logger.LogInformation("Queued command processed successfully.");
    }
}
