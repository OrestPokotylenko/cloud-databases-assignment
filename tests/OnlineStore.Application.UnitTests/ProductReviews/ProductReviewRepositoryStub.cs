using OnlineStore.Application.ProductReviews.Abstractions;
using OnlineStore.Domain.ProductReviews;

namespace OnlineStore.Application.UnitTests.ProductReviews;

internal class ProductReviewRepositoryStub : IProductReviewRepository
{
    public ProductReviewPage Page { get; set; } = new([], null);
    public bool DeleteResult { get; set; } = true;
    public ProductReview? AddedReview { get; private set; }
    public Guid? RequestedProductPublicId { get; private set; }
    public Guid? RequestedReviewPublicId { get; private set; }
    public int? RequestedPageSize { get; private set; }
    public string? RequestedContinuationToken { get; private set; }
    public CancellationToken ReceivedCancellationToken { get; private set; }

    public Task AddAsync(
        ProductReview review,
        CancellationToken ct)
    {
        AddedReview = review;
        ReceivedCancellationToken = ct;
        return Task.CompletedTask;
    }

    public Task<ProductReviewPage> GetByProductAsync(
        Guid productPublicId,
        int pageSize,
        string? continuationToken,
        CancellationToken ct)
    {
        RequestedProductPublicId = productPublicId;
        RequestedPageSize = pageSize;
        RequestedContinuationToken = continuationToken;
        ReceivedCancellationToken = ct;
        return Task.FromResult(Page);
    }

    public Task<bool> DeleteAsync(
        Guid productPublicId,
        Guid reviewPublicId,
        CancellationToken ct)
    {
        RequestedProductPublicId = productPublicId;
        RequestedReviewPublicId = reviewPublicId;
        ReceivedCancellationToken = ct;
        return Task.FromResult(DeleteResult);
    }
}
