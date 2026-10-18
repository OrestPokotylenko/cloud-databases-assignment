using OnlineStore.Application.Abstractions.CQRS;

namespace OnlineStore.Application.ProductReviews.DeleteProductReview;

public record DeleteProductReviewCommand(
    Guid ProductPublicId,
    Guid ReviewPublicId) : ICommand<DeleteProductReviewResult>;
