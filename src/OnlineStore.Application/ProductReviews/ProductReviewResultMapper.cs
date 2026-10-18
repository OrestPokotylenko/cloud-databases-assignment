using OnlineStore.Domain.ProductReviews;

namespace OnlineStore.Application.ProductReviews;

public static class ProductReviewResultMapper
{
    public static ProductReviewResult Map(ProductReview review) =>
        new(
            review.PublicId,
            review.Comment,
            review.Rating,
            review.CreatedAt);
}
