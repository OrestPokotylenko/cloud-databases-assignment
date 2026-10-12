using OnlineStore.Application.Abstractions.CQRS;

namespace OnlineStore.Application.Orders.GetOrderById;

public record GetOrderByIdQuery(Guid PublicId)
    : IQuery<GetOrderByIdResult?>;
