namespace OnlineStore.Application.Orders.Messages;

public record OrderItemMessage(
    Guid ProductPublicId,
    int Quantity);
