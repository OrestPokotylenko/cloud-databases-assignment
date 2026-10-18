using OnlineStore.Domain.ProductReviews;

namespace OnlineStore.Application.ProductReviews.Abstractions;

public interface IProductReviewRepository
{
    Task AddAsync(
        ProductReview review,
        CancellationToken ct);

    Task<ProductReviewPage> GetByProductAsync(
        Guid productPublicId,
        int pageSize,
        string? continuationToken,
        CancellationToken ct);

    Task<bool> DeleteAsync(
        Guid productPublicId,
        Guid reviewPublicId,
        CancellationToken ct);
}
