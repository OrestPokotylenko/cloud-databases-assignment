using OnlineStore.Application.Products.Abstractions;
using OnlineStore.Application.Products.GetProducts;

using Xunit;

namespace OnlineStore.Application.UnitTests.Products;

public sealed class GetProductsHandlerTests
{
    [Fact]
    public async Task HandleAsync_ValidPage_ReturnsMappedPage()
    {
        var productId = Guid.NewGuid();
        var createdAt = DateTimeOffset.UtcNow;
        var repository = new ProductQueryRepositoryStub
        {
            Page = new ProductPageData(
                [new ProductData(productId, createdAt, "Widget", null, 9.99m, 4)],
                21)
        };
        var handler = new GetProductsHandler(repository);

        var result = await handler.HandleAsync(
            new GetProductsQuery(2, 10),
            TestContext.Current.CancellationToken);

        var product = Assert.Single(result.Products);
        Assert.Equal(productId, product.PublicId);
        Assert.Equal(createdAt, product.CreatedAt);
        Assert.Equal(2, result.PageNumber);
        Assert.Equal(10, result.PageSize);
        Assert.Equal(21, result.TotalCount);
        Assert.Equal(3, result.TotalPages);
        Assert.Equal(10, repository.RequestedSkip);
        Assert.Equal(10, repository.RequestedTake);
    }

    [Theory]
    [InlineData(0, 20)]
    [InlineData(1, 0)]
    [InlineData(1, 101)]
    [InlineData(int.MaxValue, 100)]
    public async Task HandleAsync_InvalidPaging_RejectsQuery(
        int pageNumber,
        int pageSize)
    {
        var repository = new ProductQueryRepositoryStub();
        var handler = new GetProductsHandler(repository);

        await Assert.ThrowsAsync<ArgumentOutOfRangeException>(() =>
            handler.HandleAsync(
                new GetProductsQuery(pageNumber, pageSize),
                TestContext.Current.CancellationToken));

        Assert.Null(repository.RequestedSkip);
    }
}
