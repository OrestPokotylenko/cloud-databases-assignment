using Microsoft.Azure.Functions.Worker;

using OnlineStore.Application.Orders.Messages;
using OnlineStore.Functions.Messaging;

namespace OnlineStore.Functions.Orders;

public class CreateOrderFunction(
    QueueCommandProcessor processor)
{
    [Function(nameof(CreateOrderFunction))]
    public Task RunAsync(
        [QueueTrigger(
            "%Queues:CreateOrderQueueName%",
            Connection = "Queues:ConnectionString")]
        string message,
        CancellationToken ct) =>
        processor.ProcessAsync<CreateOrderMessage>(message, ct);
}
