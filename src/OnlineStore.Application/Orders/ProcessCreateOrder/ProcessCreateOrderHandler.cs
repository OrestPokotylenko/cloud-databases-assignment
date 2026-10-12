using OnlineStore.Application.Abstractions.CQRS;
using OnlineStore.Application.Abstractions.Messaging;
using OnlineStore.Application.Orders.Abstractions;
using OnlineStore.Application.Orders.Messages;

namespace OnlineStore.Application.Orders.ProcessCreateOrder;

public class ProcessCreateOrderHandler(
    IOrderCommandRepository orderRepository)
    : IHandler<CreateOrderMessage, QueuedCommandResult>
{
    public async Task<QueuedCommandResult> HandleAsync(
        CreateOrderMessage request,
        CancellationToken ct)
    {
        await orderRepository.CreateAsync(request, ct);
        return new QueuedCommandResult();
    }
}
