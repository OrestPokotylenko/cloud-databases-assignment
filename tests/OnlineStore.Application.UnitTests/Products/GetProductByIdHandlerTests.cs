using OnlineStore.Application.Products.Abstractions;
using OnlineStore.Application.Products.GetProductById;

using Xunit;

namespace OnlineStore.Application.UnitTests.Products;

public class GetProductByIdHandlerTests
{
    [Fact]
    public async Task HandleAsync_ExistingProduct_ReturnsProduct()
    {
        var publicId = Guid.NewGuid();
        var createdAt = DateTimeOffset.UtcNow;
        var repository = new ProductQueryRepositoryStub
        {
            Product = new ProductData(
                publicId,
                createdAt,
                "Widget",
                "Useful",
                5m,
                2)
        };
        var handler = new GetProductByIdHandler(repository);

        var result = await handler.HandleAsync(
            new GetProductByIdQuery(publicId),
            TestContext.Current.CancellationToken);

        Assert.NotNull(result);
        Assert.Equal(publicId, result.PublicId);
        Assert.Equal(createdAt, result.CreatedAt);
        Assert.Equal("Widget", result.Name);
        Assert.Equal(publicId, repository.RequestedPublicId);
    }

    [Fact]
    public async Task HandleAsync_MissingProduct_ReturnsNull()
    {
        var repository = new ProductQueryRepositoryStub();
        var handler = new GetProductByIdHandler(repository);

        var result = await handler.HandleAsync(
            new GetProductByIdQuery(Guid.NewGuid()),
            TestContext.Current.CancellationToken);

        Assert.Null(result);
    }
}
