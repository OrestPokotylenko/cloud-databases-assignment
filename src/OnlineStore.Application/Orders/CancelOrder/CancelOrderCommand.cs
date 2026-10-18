using OnlineStore.Application.Abstractions.CQRS;
using OnlineStore.Application.Orders.Actions;

namespace OnlineStore.Application.Orders.CancelOrder;

public record CancelOrderCommand(Guid PublicId)
    : ICommand<OrderActionResult>;
