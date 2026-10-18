using OnlineStore.Application.ProductReviews.Abstractions;
using OnlineStore.Application.ProductReviews.GetProductReviews;
using OnlineStore.Application.UnitTests.Products;
using OnlineStore.Domain.ProductReviews;

using Xunit;

namespace OnlineStore.Application.UnitTests.ProductReviews;

public class GetProductReviewsHandlerTests
{
    [Fact]
    public async Task HandleAsync_ExistingProduct_ReturnsPaginatedReviews()
    {
        var productPublicId = Guid.NewGuid();
        var reviewPublicId = Guid.NewGuid();
        var createdAt = DateTimeOffset.UtcNow;
        var repository = new ProductReviewRepositoryStub
        {
            Page = new ProductReviewPage(
                [new ProductReview(
                    reviewPublicId,
                    productPublicId,
                    "Very useful product",
                    4,
                    createdAt)],
                "next-page")
        };
        var handler = new GetProductReviewsHandler(
            new ProductQueryRepositoryStub(),
            repository);

        var result = await handler.HandleAsync(
            new GetProductReviewsQuery(productPublicId, 20, "current-page"),
            TestContext.Current.CancellationToken);

        Assert.True(result.ProductExists);
        Assert.NotNull(result.Page);
        var review = Assert.Single(result.Page.Items);
        Assert.Equal(reviewPublicId, review.PublicId);
        Assert.Equal("next-page", result.Page.ContinuationToken);
        Assert.Equal(productPublicId, repository.RequestedProductPublicId);
        Assert.Equal(20, repository.RequestedPageSize);
        Assert.Equal("current-page", repository.RequestedContinuationToken);
    }
}
