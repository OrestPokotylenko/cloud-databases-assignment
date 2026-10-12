namespace OnlineStore.Application.Orders.GetOrderById;

public record OrderItemResult(
    Guid ProductPublicId,
    string ProductName,
    decimal UnitPrice,
    int Quantity,
    decimal LineTotal);
