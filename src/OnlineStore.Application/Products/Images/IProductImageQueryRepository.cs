namespace OnlineStore.Application.Products.Images;

public interface IProductImageQueryRepository
{
    Task<bool> ProductExistsAsync(
        Guid productPublicId,
        CancellationToken ct);

    Task<ProductImageData?> GetAsync(
        Guid productPublicId,
        Guid imagePublicId,
        CancellationToken ct);
}
