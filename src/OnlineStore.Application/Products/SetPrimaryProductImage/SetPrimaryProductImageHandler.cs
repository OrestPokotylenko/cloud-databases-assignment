using OnlineStore.Application.Abstractions.CQRS;
using OnlineStore.Application.Products.Images;

namespace OnlineStore.Application.Products.SetPrimaryProductImage;

public class SetPrimaryProductImageHandler(
    IProductImageCommandRepository repository)
    : IHandler<SetPrimaryProductImageCommand, SetPrimaryProductImageResult>
{
    public async Task<SetPrimaryProductImageResult> HandleAsync(
        SetPrimaryProductImageCommand request,
        CancellationToken ct)
    {
        ArgumentOutOfRangeException.ThrowIfEqual(request.ProductPublicId, Guid.Empty);
        ArgumentOutOfRangeException.ThrowIfEqual(request.ImagePublicId, Guid.Empty);

        var updated = await repository.SetPrimaryAsync(
            request.ProductPublicId,
            request.ImagePublicId,
            ct);

        return new SetPrimaryProductImageResult(updated);
    }
}
