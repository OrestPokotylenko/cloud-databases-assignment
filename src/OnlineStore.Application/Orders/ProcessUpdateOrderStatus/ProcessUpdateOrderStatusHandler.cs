using OnlineStore.Application.Abstractions.CQRS;
using OnlineStore.Application.Abstractions.Messaging;
using OnlineStore.Application.Orders.Abstractions;
using OnlineStore.Application.Orders.Messages;

namespace OnlineStore.Application.Orders.ProcessUpdateOrderStatus;

public class ProcessUpdateOrderStatusHandler(
    IOrderCommandRepository orderRepository)
    : IHandler<UpdateOrderStatusMessage, QueuedCommandResult>
{
    public async Task<QueuedCommandResult> HandleAsync(
        UpdateOrderStatusMessage request,
        CancellationToken ct)
    {
        await orderRepository.UpdateStatusAsync(request, ct);
        return new QueuedCommandResult();
    }
}
