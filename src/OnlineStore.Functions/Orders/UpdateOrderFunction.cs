using Microsoft.Azure.Functions.Worker;

using OnlineStore.Application.Orders.Messages;
using OnlineStore.Functions.Messaging;

namespace OnlineStore.Functions.Orders;

public class UpdateOrderFunction(
    QueueCommandProcessor processor)
{
    [Function(nameof(UpdateOrderFunction))]
    public Task RunAsync(
        [QueueTrigger(
            "%Queues:OrderUpdateQueueName%",
            Connection = "Queues:ConnectionString")]
        string message,
        CancellationToken ct) =>
        processor.ProcessAsync<OrderActionMessage>(message, ct);
}
