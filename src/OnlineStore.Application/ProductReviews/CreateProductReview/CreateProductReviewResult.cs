namespace OnlineStore.Application.ProductReviews.CreateProductReview;

public record CreateProductReviewResult(
    bool ProductExists,
    ProductReviewResult? Review);
