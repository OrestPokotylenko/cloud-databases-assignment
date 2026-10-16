using OnlineStore.Application.Abstractions.CQRS;
using OnlineStore.Application.Abstractions.Messaging;
using OnlineStore.Application.Orders.Abstractions;
using OnlineStore.Application.Orders.CancelOrder;
using OnlineStore.Application.Orders.Messages;
using OnlineStore.Application.Orders.ProcessOrder;
using OnlineStore.Application.Orders.ShipOrder;

namespace OnlineStore.Application.Orders.Actions;

public class OrderActionHandler(
    IOrderQueryRepository orderRepository,
    IMessagePublisher<OrderActionMessage> publisher,
    TimeProvider timeProvider)
    : IHandler<ProcessOrderCommand, OrderActionResult>,
      IHandler<ShipOrderCommand, OrderActionResult>,
      IHandler<CancelOrderCommand, OrderActionResult>
{
    public Task<OrderActionResult> HandleAsync(
        ProcessOrderCommand request,
        CancellationToken ct) =>
        EnqueueAsync(request.PublicId, OrderAction.Process, ct);

    public Task<OrderActionResult> HandleAsync(
        ShipOrderCommand request,
        CancellationToken ct) =>
        EnqueueAsync(request.PublicId, OrderAction.Ship, ct);

    public Task<OrderActionResult> HandleAsync(
        CancelOrderCommand request,
        CancellationToken ct) =>
        EnqueueAsync(request.PublicId, OrderAction.Cancel, ct);

    private async Task<OrderActionResult> EnqueueAsync(
        Guid publicId,
        OrderAction action,
        CancellationToken ct)
    {
        if (!await orderRepository.ExistsAsync(publicId, ct))
        {
            return new OrderActionResult(false);
        }

        await publisher.PublishAsync(
            new OrderActionMessage(
                Guid.NewGuid(),
                Guid.NewGuid(),
                timeProvider.GetUtcNow(),
                publicId,
                action),
            ct);

        return new OrderActionResult(true);
    }
}
