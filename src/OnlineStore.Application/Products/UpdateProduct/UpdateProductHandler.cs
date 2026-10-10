using OnlineStore.Application.Abstractions.CQRS;
using OnlineStore.Application.Products.Abstractions;
using OnlineStore.Domain.Products;

namespace OnlineStore.Application.Products.UpdateProduct;

public sealed class UpdateProductHandler(
    IProductCommandRepository productRepository)
    : IHandler<UpdateProductCommand, UpdateProductResult>
{
    public async Task<UpdateProductResult> HandleAsync(
        UpdateProductCommand request,
        CancellationToken ct)
    {
        ArgumentOutOfRangeException.ThrowIfEqual(request.PublicId, Guid.Empty);

        var details = new ProductDetails(
            request.Name,
            request.Description,
            request.Price,
            request.StockQuantity);

        var productUpdate = new ProductUpdate(
            request.PublicId,
            details);

        var updated = await productRepository.UpdateAsync(productUpdate, ct);

        return new UpdateProductResult(updated);
    }
}
