using OnlineStore.Application.Abstractions.CQRS;

namespace OnlineStore.Application.Products.GetProductById;

public record GetProductByIdQuery(Guid PublicId)
    : IQuery<GetProductByIdResult?>;
