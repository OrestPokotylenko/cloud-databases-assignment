using OnlineStore.Application.Abstractions.CQRS;
using OnlineStore.Application.ProductReviews.Abstractions;
using OnlineStore.Application.Products.Abstractions;

namespace OnlineStore.Application.ProductReviews.GetProductReviews;

public class GetProductReviewsHandler(
    IProductQueryRepository productRepository,
    IProductReviewRepository reviewRepository)
    : IHandler<GetProductReviewsQuery, GetProductReviewsResult>
{
    public async Task<GetProductReviewsResult> HandleAsync(
        GetProductReviewsQuery request,
        CancellationToken ct)
    {
        ArgumentOutOfRangeException.ThrowIfEqual(
            request.ProductPublicId,
            Guid.Empty);

        if (request.PageSize is < 1 or > GetProductReviewsQuery.MaximumPageSize)
        {
            throw new ArgumentOutOfRangeException(
                nameof(request.PageSize),
                $"The page size must be between 1 and {GetProductReviewsQuery.MaximumPageSize}.");
        }

        if (!await productRepository.ExistsAsync(request.ProductPublicId, ct))
        {
            return new GetProductReviewsResult(false, null);
        }

        var page = await reviewRepository.GetByProductAsync(
            request.ProductPublicId,
            request.PageSize,
            request.ContinuationToken,
            ct);

        return new GetProductReviewsResult(
            true,
            new ProductReviewPageResult(
                page.Items
                    .Select(ProductReviewResultMapper.Map)
                    .ToArray(),
                page.ContinuationToken));
    }
}
