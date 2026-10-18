using OnlineStore.Application.Abstractions.CQRS;
using OnlineStore.Application.ProductReviews.Abstractions;

namespace OnlineStore.Application.ProductReviews.DeleteProductReview;

public class DeleteProductReviewHandler(
    IProductReviewRepository reviewRepository)
    : IHandler<DeleteProductReviewCommand, DeleteProductReviewResult>
{
    public async Task<DeleteProductReviewResult> HandleAsync(
        DeleteProductReviewCommand request,
        CancellationToken ct)
    {
        ArgumentOutOfRangeException.ThrowIfEqual(
            request.ProductPublicId,
            Guid.Empty);
        ArgumentOutOfRangeException.ThrowIfEqual(
            request.ReviewPublicId,
            Guid.Empty);

        var deleted = await reviewRepository.DeleteAsync(
            request.ProductPublicId,
            request.ReviewPublicId,
            ct);

        return new DeleteProductReviewResult(deleted);
    }
}
