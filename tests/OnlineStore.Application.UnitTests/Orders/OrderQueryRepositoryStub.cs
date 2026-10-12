using OnlineStore.Application.Orders.Abstractions;
using OnlineStore.Domain.Orders;

namespace OnlineStore.Application.UnitTests.Orders;

internal class OrderQueryRepositoryStub : IOrderQueryRepository
{
    public OrderData? Order { get; set; }
    public OrderPageData Page { get; set; } = new([], 0);
    public OrderStatus? Status { get; set; }

    public Task<OrderData?> GetByPublicIdAsync(Guid publicId, CancellationToken ct) =>
        Task.FromResult(Order);

    public Task<OrderPageData> GetPageAsync(int skip, int take, CancellationToken ct) =>
        Task.FromResult(Page);

    public Task<OrderStatus?> GetStatusAsync(Guid publicId, CancellationToken ct) =>
        Task.FromResult(Status);
}
