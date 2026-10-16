using OnlineStore.Application.Abstractions.CQRS;
using OnlineStore.Application.Orders.Abstractions;

namespace OnlineStore.Application.Orders.GetOrders;

public class GetOrdersHandler(
    IOrderQueryRepository orderRepository)
    : IHandler<GetOrdersQuery, GetOrdersResult>
{
    public async Task<GetOrdersResult> HandleAsync(
        GetOrdersQuery request,
        CancellationToken ct)
    {
        if (request.PageNumber < 1)
        {
            throw new ArgumentOutOfRangeException(nameof(request.PageNumber));
        }

        if (request.PageSize is < 1 or > 100)
        {
            throw new ArgumentOutOfRangeException(nameof(request.PageSize));
        }

        var skip = checked((request.PageNumber - 1) * request.PageSize);
        var page = await orderRepository.GetPageAsync(skip, request.PageSize, ct);
        var totalPages = (int)Math.Ceiling(page.TotalCount / (double)request.PageSize);

        return new GetOrdersResult(
            request.PageNumber,
            request.PageSize,
            page.TotalCount,
            totalPages,
            page.Orders.Select(order => new OrderListItem(
                order.PublicId,
                order.OrderDate,
                order.Status,
                order.CustomerFullName,
                order.TotalAmount)).ToList());
    }
}
