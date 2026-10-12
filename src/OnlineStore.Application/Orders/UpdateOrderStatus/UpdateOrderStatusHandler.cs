using OnlineStore.Application.Abstractions.CQRS;
using OnlineStore.Application.Abstractions.Messaging;
using OnlineStore.Application.Orders.Abstractions;
using OnlineStore.Application.Orders.Messages;
using OnlineStore.Domain.Orders;

namespace OnlineStore.Application.Orders.UpdateOrderStatus;

public class UpdateOrderStatusHandler(
    IOrderQueryRepository orderRepository,
    IMessagePublisher<UpdateOrderStatusMessage> publisher,
    TimeProvider timeProvider)
    : IHandler<UpdateOrderStatusCommand, UpdateOrderStatusResult>
{
    public async Task<UpdateOrderStatusResult> HandleAsync(
        UpdateOrderStatusCommand request,
        CancellationToken ct)
    {
        var normalizedStatus = request.Status?.Trim();
        var isNamedStatus = Enum.GetNames<OrderStatus>()
            .Any(name => string.Equals(
                name,
                normalizedStatus,
                StringComparison.OrdinalIgnoreCase));

        if (!isNamedStatus ||
            !Enum.TryParse<OrderStatus>(normalizedStatus, true, out var status) ||
            status == OrderStatus.Rejected)
        {
            throw new ArgumentException(
                "Status must be Pending, Processing, Shipped, or Cancelled.",
                nameof(request.Status));
        }

        var currentStatus = await orderRepository.GetStatusAsync(request.PublicId, ct);
        if (currentStatus is null)
        {
            return new UpdateOrderStatusResult(false, null);
        }

        if (!CanTransition(currentStatus.Value, status))
        {
            throw new ArgumentException(
                $"Order status cannot change from {currentStatus} to {status}.",
                nameof(request.Status));
        }

        await publisher.PublishAsync(
            new UpdateOrderStatusMessage(
                Guid.NewGuid(),
                Guid.NewGuid(),
                timeProvider.GetUtcNow(),
                request.PublicId,
                status),
            ct);

        return new UpdateOrderStatusResult(true, status);
    }

    private static bool CanTransition(
        OrderStatus current,
        OrderStatus requested) =>
        requested == current ||
        current == OrderStatus.Pending && requested is OrderStatus.Processing or OrderStatus.Cancelled ||
        current == OrderStatus.Processing && requested is OrderStatus.Shipped or OrderStatus.Cancelled;
}
