namespace OnlineStore.Application.Products.Abstractions;

public interface IProductQueryRepository
{
    Task<bool> ExistsAsync(
        Guid publicId,
        CancellationToken ct);

    Task<ProductData?> GetByPublicIdAsync(
        Guid publicId,
        CancellationToken ct);

    Task<ProductPageData> GetPageAsync(
        int skip,
        int take,
        CancellationToken ct);
}
