using OnlineStore.Application.Abstractions.CQRS;

namespace OnlineStore.Application.Orders.GetOrders;

public record GetOrdersQuery(
    int PageNumber = 1,
    int PageSize = 20)
    : IQuery<GetOrdersResult>;
