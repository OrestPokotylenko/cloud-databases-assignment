using OnlineStore.Application.Abstractions.CQRS;
using OnlineStore.Application.Abstractions.Messaging;
using OnlineStore.Application.Orders.Abstractions;
using OnlineStore.Application.Orders.Messages;

namespace OnlineStore.Application.Orders.ProcessOrderAction;

public class ProcessOrderActionHandler(
    IOrderCommandRepository orderRepository)
    : IHandler<OrderActionMessage, QueuedCommandResult>
{
    public async Task<QueuedCommandResult> HandleAsync(
        OrderActionMessage request,
        CancellationToken ct)
    {
        await orderRepository.ApplyActionAsync(request, ct);
        return new QueuedCommandResult();
    }
}
