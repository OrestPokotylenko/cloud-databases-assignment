using OnlineStore.Application.Abstractions.CQRS;
using OnlineStore.Application.Orders.Actions;

namespace OnlineStore.Application.Orders.ProcessOrder;

public record ProcessOrderCommand(Guid PublicId)
    : ICommand<OrderActionResult>;
