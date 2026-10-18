using OnlineStore.Application.Abstractions.CQRS;
using OnlineStore.Application.Products.Images;

namespace OnlineStore.Application.Products.DeleteProductImage;

public class DeleteProductImageHandler(
    IProductImageCommandRepository commandRepository,
    IProductImageQueryRepository queryRepository,
    IProductImageStorage storage)
    : IHandler<DeleteProductImageCommand, DeleteProductImageResult>
{
    public async Task<DeleteProductImageResult> HandleAsync(
        DeleteProductImageCommand request,
        CancellationToken ct)
    {
        ArgumentOutOfRangeException.ThrowIfEqual(request.ProductPublicId, Guid.Empty);
        ArgumentOutOfRangeException.ThrowIfEqual(request.ImagePublicId, Guid.Empty);

        var image = await queryRepository.GetAsync(
            request.ProductPublicId,
            request.ImagePublicId,
            ct);
        if (image is null)
        {
            return new DeleteProductImageResult(false);
        }

        await storage.DeleteAsync(image.BlobName, ct);
        var deleted = await commandRepository.DeleteAsync(
            request.ProductPublicId,
            request.ImagePublicId,
            ct);

        return new DeleteProductImageResult(deleted);
    }
}
