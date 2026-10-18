using OnlineStore.Application.Abstractions.CQRS;
using OnlineStore.Application.Orders.Actions;

namespace OnlineStore.Application.Orders.ShipOrder;

public record ShipOrderCommand(Guid PublicId)
    : ICommand<OrderActionResult>;
