using OnlineStore.Domain.Orders;

namespace OnlineStore.Application.Orders.UpdateOrderStatus;

public record UpdateOrderStatusResult(
    bool Accepted,
    OrderStatus? Status);
