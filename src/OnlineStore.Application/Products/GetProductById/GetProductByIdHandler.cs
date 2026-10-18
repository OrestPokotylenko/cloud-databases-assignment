using OnlineStore.Application.Abstractions.CQRS;
using OnlineStore.Application.Products.Abstractions;
using OnlineStore.Application.Products.Images;

namespace OnlineStore.Application.Products.GetProductById;

public class GetProductByIdHandler(
    IProductQueryRepository productRepository,
    IProductImageStorage imageStorage)
    : IHandler<GetProductByIdQuery, GetProductByIdResult?>
{
    public async Task<GetProductByIdResult?> HandleAsync(
        GetProductByIdQuery request,
        CancellationToken ct)
    {
        ArgumentOutOfRangeException.ThrowIfEqual(request.PublicId, Guid.Empty);

        var product = await productRepository.GetByPublicIdAsync(
            request.PublicId,
            ct);

        return product is null
            ? null
            : new GetProductByIdResult(
                product.PublicId,
                product.CreatedAt,
                product.Name,
                product.Description,
                product.Price,
                product.StockQuantity,
                product.Images
                    .Select(image => ProductImageResultMapper.Map(
                        image,
                        imageStorage))
                    .ToArray());
    }
}
