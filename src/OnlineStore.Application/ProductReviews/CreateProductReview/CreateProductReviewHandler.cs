using OnlineStore.Application.Abstractions.CQRS;
using OnlineStore.Application.ProductReviews.Abstractions;
using OnlineStore.Application.Products.Abstractions;
using OnlineStore.Domain.ProductReviews;

namespace OnlineStore.Application.ProductReviews.CreateProductReview;

public class CreateProductReviewHandler(
    IProductQueryRepository productRepository,
    IProductReviewRepository reviewRepository,
    TimeProvider timeProvider)
    : IHandler<CreateProductReviewCommand, CreateProductReviewResult>
{
    public async Task<CreateProductReviewResult> HandleAsync(
        CreateProductReviewCommand request,
        CancellationToken ct)
    {
        ArgumentOutOfRangeException.ThrowIfEqual(
            request.ProductPublicId,
            Guid.Empty);

        if (!await productRepository.ExistsAsync(request.ProductPublicId, ct))
        {
            return new CreateProductReviewResult(false, null);
        }

        var review = new ProductReview(
            Guid.NewGuid(),
            request.ProductPublicId,
            request.Comment,
            request.Rating,
            timeProvider.GetUtcNow());

        await reviewRepository.AddAsync(review, ct);

        return new CreateProductReviewResult(
            true,
            ProductReviewResultMapper.Map(review));
    }
}
