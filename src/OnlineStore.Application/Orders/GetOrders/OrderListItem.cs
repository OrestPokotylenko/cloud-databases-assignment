using OnlineStore.Domain.Orders;

namespace OnlineStore.Application.Orders.GetOrders;

public record OrderListItem(
    Guid PublicId,
    DateTimeOffset OrderDate,
    OrderStatus Status,
    string CustomerFullName,
    decimal TotalAmount);
