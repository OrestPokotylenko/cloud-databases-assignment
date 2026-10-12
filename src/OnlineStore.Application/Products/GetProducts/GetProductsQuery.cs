using OnlineStore.Application.Abstractions.CQRS;

namespace OnlineStore.Application.Products.GetProducts;

public record GetProductsQuery(
    int PageNumber = 1,
    int PageSize = 20) : IQuery<GetProductsResult>;
