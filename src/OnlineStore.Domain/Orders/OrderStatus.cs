namespace OnlineStore.Domain.Orders;

public enum OrderStatus
{
    Pending,
    Processing,
    Shipped,
    Cancelled,
    Rejected
}
