using OnlineStore.Application.Abstractions.CQRS;

namespace OnlineStore.Application.ProductReviews.CreateProductReview;

public record CreateProductReviewCommand(
    Guid ProductPublicId,
    string Comment,
    int? Rating) : ICommand<CreateProductReviewResult>;
