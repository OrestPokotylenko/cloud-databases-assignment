namespace OnlineStore.Application.Orders.Abstractions;

public record OrderItemData(
    Guid ProductPublicId,
    string ProductName,
    decimal UnitPrice,
    int Quantity,
    decimal LineTotal);
