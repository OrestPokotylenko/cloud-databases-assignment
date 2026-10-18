using OnlineStore.Application.ProductReviews.CreateProductReview;
using OnlineStore.Application.UnitTests.Products;

using Xunit;

namespace OnlineStore.Application.UnitTests.ProductReviews;

public class CreateProductReviewHandlerTests
{
    [Fact]
    public async Task HandleAsync_ValidReview_CreatesReview()
    {
        var productPublicId = Guid.NewGuid();
        var createdAt = new DateTimeOffset(2026, 10, 18, 18, 0, 0, TimeSpan.Zero);
        var productRepository = new ProductQueryRepositoryStub();
        var reviewRepository = new ProductReviewRepositoryStub();
        var handler = new CreateProductReviewHandler(
            productRepository,
            reviewRepository,
            new FixedTimeProvider(createdAt));

        var result = await handler.HandleAsync(
            new CreateProductReviewCommand(
                productPublicId,
                "  Very useful product  ",
                4),
            TestContext.Current.CancellationToken);

        Assert.True(result.ProductExists);
        Assert.NotNull(result.Review);
        Assert.NotEqual(Guid.Empty, result.Review.PublicId);
        Assert.Equal("Very useful product", result.Review.Comment);
        Assert.Equal(4, result.Review.Rating);
        Assert.Equal(createdAt, result.Review.CreatedAt);
        Assert.NotNull(reviewRepository.AddedReview);
        Assert.Equal(result.Review.PublicId, reviewRepository.AddedReview.PublicId);
        Assert.Equal(productPublicId, reviewRepository.AddedReview.ProductPublicId);
    }

    [Fact]
    public async Task HandleAsync_EmptyComment_RejectsReview()
    {
        var handler = new CreateProductReviewHandler(
            new ProductQueryRepositoryStub(),
            new ProductReviewRepositoryStub(),
            new FixedTimeProvider(DateTimeOffset.UtcNow));

        await Assert.ThrowsAsync<ArgumentException>(() =>
            handler.HandleAsync(
                new CreateProductReviewCommand(Guid.NewGuid(), " ", null),
                TestContext.Current.CancellationToken));
    }

    [Theory]
    [InlineData(0)]
    [InlineData(6)]
    public async Task HandleAsync_InvalidRating_RejectsReview(int rating)
    {
        var handler = new CreateProductReviewHandler(
            new ProductQueryRepositoryStub(),
            new ProductReviewRepositoryStub(),
            new FixedTimeProvider(DateTimeOffset.UtcNow));

        await Assert.ThrowsAsync<ArgumentOutOfRangeException>(() =>
            handler.HandleAsync(
                new CreateProductReviewCommand(
                    Guid.NewGuid(),
                    "Useful product",
                    rating),
                TestContext.Current.CancellationToken));
    }

    [Fact]
    public async Task HandleAsync_MissingProduct_DoesNotCreateReview()
    {
        var productRepository = new ProductQueryRepositoryStub
        {
            ProductExists = false
        };
        var reviewRepository = new ProductReviewRepositoryStub();
        var handler = new CreateProductReviewHandler(
            productRepository,
            reviewRepository,
            new FixedTimeProvider(DateTimeOffset.UtcNow));

        var result = await handler.HandleAsync(
            new CreateProductReviewCommand(
                Guid.NewGuid(),
                "Useful product",
                4),
            TestContext.Current.CancellationToken);

        Assert.False(result.ProductExists);
        Assert.Null(result.Review);
        Assert.Null(reviewRepository.AddedReview);
    }
}
