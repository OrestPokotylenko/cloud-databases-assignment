using OnlineStore.Application.Abstractions.CQRS;

namespace OnlineStore.Application.Products.GetProductById;

public sealed record GetProductByIdQuery(Guid PublicId)
    : IQuery<GetProductByIdResult?>;
