using OnlineStore.Application.Abstractions.CQRS;
using OnlineStore.Application.Products.Abstractions;

namespace OnlineStore.Application.Products.DeleteProduct;

public sealed class DeleteProductHandler(
    IProductCommandRepository productRepository)
    : IHandler<DeleteProductCommand, DeleteProductResult>
{
    public async Task<DeleteProductResult> HandleAsync(
        DeleteProductCommand request,
        CancellationToken ct)
    {
        ArgumentOutOfRangeException.ThrowIfEqual(request.PublicId, Guid.Empty);

        var deleted = await productRepository.DeleteAsync(request.PublicId, ct);

        return new DeleteProductResult(deleted);
    }
}
