using OnlineStore.Domain.Orders;

namespace OnlineStore.Application.Orders.Abstractions;

public interface IOrderQueryRepository
{
    Task<OrderData?> GetByPublicIdAsync(
        Guid publicId,
        CancellationToken ct);

    Task<OrderPageData> GetPageAsync(
        int skip,
        int take,
        CancellationToken ct);

    Task<OrderStatus?> GetStatusAsync(
        Guid publicId,
        CancellationToken ct);
}
