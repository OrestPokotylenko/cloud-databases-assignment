using OnlineStore.Application.Abstractions.CQRS;

namespace OnlineStore.Application.ProductReviews.GetProductReviews;

public record GetProductReviewsQuery(
    Guid ProductPublicId,
    int PageSize,
    string? ContinuationToken) : IQuery<GetProductReviewsResult>
{
    public const int MaximumPageSize = 100;
}
