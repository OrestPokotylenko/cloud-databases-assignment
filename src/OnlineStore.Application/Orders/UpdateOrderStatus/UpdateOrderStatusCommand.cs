using OnlineStore.Application.Abstractions.CQRS;

namespace OnlineStore.Application.Orders.UpdateOrderStatus;

public record UpdateOrderStatusCommand(
    Guid PublicId,
    string Status)
    : ICommand<UpdateOrderStatusResult>;
