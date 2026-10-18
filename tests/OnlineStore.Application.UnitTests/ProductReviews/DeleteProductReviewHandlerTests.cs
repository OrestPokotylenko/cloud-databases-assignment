using OnlineStore.Application.ProductReviews.DeleteProductReview;

using Xunit;

namespace OnlineStore.Application.UnitTests.ProductReviews;

public class DeleteProductReviewHandlerTests
{
    [Fact]
    public async Task HandleAsync_ExistingReview_DeletesReview()
    {
        var productPublicId = Guid.NewGuid();
        var reviewPublicId = Guid.NewGuid();
        var repository = new ProductReviewRepositoryStub();
        var handler = new DeleteProductReviewHandler(repository);

        var result = await handler.HandleAsync(
            new DeleteProductReviewCommand(
                productPublicId,
                reviewPublicId),
            TestContext.Current.CancellationToken);

        Assert.True(result.Deleted);
        Assert.Equal(productPublicId, repository.RequestedProductPublicId);
        Assert.Equal(reviewPublicId, repository.RequestedReviewPublicId);
    }
}
