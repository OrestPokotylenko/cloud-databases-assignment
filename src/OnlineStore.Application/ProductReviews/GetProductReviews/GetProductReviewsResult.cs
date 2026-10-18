namespace OnlineStore.Application.ProductReviews.GetProductReviews;

public record GetProductReviewsResult(
    bool ProductExists,
    ProductReviewPageResult? Page);
