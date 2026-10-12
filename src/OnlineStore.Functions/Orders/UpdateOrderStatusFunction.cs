using Microsoft.Azure.Functions.Worker;

using OnlineStore.Application.Orders.Messages;
using OnlineStore.Functions.Messaging;

namespace OnlineStore.Functions.Orders;

public class UpdateOrderStatusFunction(
    QueueCommandProcessor processor)
{
    [Function(nameof(UpdateOrderStatusFunction))]
    public Task RunAsync(
        [QueueTrigger(
            "%Queues:UpdateOrderStatusQueueName%",
            Connection = "Queues:ConnectionString")]
        string message,
        CancellationToken ct) =>
        processor.ProcessAsync<UpdateOrderStatusMessage>(message, ct);
}
