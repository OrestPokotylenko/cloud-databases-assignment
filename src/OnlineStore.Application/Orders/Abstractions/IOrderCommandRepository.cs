using OnlineStore.Application.Orders.Messages;

namespace OnlineStore.Application.Orders.Abstractions;

public interface IOrderCommandRepository
{
    Task CreateAsync(
        CreateOrderMessage message,
        CancellationToken ct);

    Task UpdateStatusAsync(
        UpdateOrderStatusMessage message,
        CancellationToken ct);
}
