namespace OnlineStore.Application.Products.Images;

public interface IProductImageCommandRepository
{
    Task<ProductImageData?> AddAsync(
        Guid productPublicId,
        Guid imagePublicId,
        string blobName,
        string contentType,
        long sizeBytes,
        CancellationToken ct);

    Task<bool> DeleteAsync(
        Guid productPublicId,
        Guid imagePublicId,
        CancellationToken ct);

    Task<bool> SetPrimaryAsync(
        Guid productPublicId,
        Guid imagePublicId,
        CancellationToken ct);
}
