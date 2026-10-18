using OnlineStore.Application.Abstractions.CQRS;

namespace OnlineStore.Application.Products.SetPrimaryProductImage;

public record SetPrimaryProductImageCommand(
    Guid ProductPublicId,
    Guid ImagePublicId)
    : ICommand<SetPrimaryProductImageResult>;
