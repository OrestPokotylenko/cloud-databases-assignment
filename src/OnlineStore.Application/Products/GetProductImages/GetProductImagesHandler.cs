using OnlineStore.Application.Abstractions.CQRS;
using OnlineStore.Application.Products.Images;

namespace OnlineStore.Application.Products.GetProductImages;

public class GetProductImagesHandler(
    IProductImageQueryRepository repository,
    IProductImageStorage storage)
    : IHandler<GetProductImagesQuery, GetProductImagesResult>
{
    public async Task<GetProductImagesResult> HandleAsync(
        GetProductImagesQuery request,
        CancellationToken ct)
    {
        ArgumentOutOfRangeException.ThrowIfEqual(request.ProductPublicId, Guid.Empty);

        var data = await repository.GetAllAsync(request.ProductPublicId, ct);
        return new GetProductImagesResult(
            data.ProductExists,
            data.Images
                .Select(image => ProductImageResultMapper.Map(image, storage))
                .ToList());
    }
}
