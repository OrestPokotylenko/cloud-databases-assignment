using OnlineStore.Domain.ProductReviews;

namespace OnlineStore.Application.ProductReviews.Abstractions;

public record ProductReviewPage(
    IReadOnlyList<ProductReview> Items,
    string? ContinuationToken);
