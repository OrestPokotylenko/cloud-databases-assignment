namespace OnlineStore.Application.Orders.GetOrders;

public record GetOrdersResult(
    int PageNumber,
    int PageSize,
    int TotalCount,
    int TotalPages,
    IReadOnlyList<OrderListItem> Orders);
