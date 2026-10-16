using OnlineStore.Application.Orders.Messages;

namespace OnlineStore.Application.Orders.Abstractions;

public interface IOrderCommandRepository
{
    Task CreateAsync(
        CreateOrderMessage message,
        CancellationToken ct);

    Task ApplyActionAsync(
        OrderActionMessage message,
        CancellationToken ct);
}
