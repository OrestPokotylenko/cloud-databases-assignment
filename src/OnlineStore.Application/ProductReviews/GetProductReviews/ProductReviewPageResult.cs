namespace OnlineStore.Application.ProductReviews.GetProductReviews;

public record ProductReviewPageResult(
    IReadOnlyList<ProductReviewResult> Items,
    string? ContinuationToken);
