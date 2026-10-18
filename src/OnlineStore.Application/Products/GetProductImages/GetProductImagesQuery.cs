using OnlineStore.Application.Abstractions.CQRS;

namespace OnlineStore.Application.Products.GetProductImages;

public record GetProductImagesQuery(Guid ProductPublicId)
    : IQuery<GetProductImagesResult>;
