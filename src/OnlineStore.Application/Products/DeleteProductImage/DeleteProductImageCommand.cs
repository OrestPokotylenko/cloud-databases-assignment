using OnlineStore.Application.Abstractions.CQRS;

namespace OnlineStore.Application.Products.DeleteProductImage;

public record DeleteProductImageCommand(
    Guid ProductPublicId,
    Guid ImagePublicId)
    : ICommand<DeleteProductImageResult>;
