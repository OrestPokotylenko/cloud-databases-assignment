using OnlineStore.Application.Products.DeleteProduct;

using Xunit;

namespace OnlineStore.Application.UnitTests.Products;

public class DeleteProductHandlerTests
{
    [Fact]
    public async Task HandleAsync_ExistingProduct_ReturnsDeleted()
    {
        var repository = new ProductCommandRepositoryStub { DeleteResult = true };
        var handler = new DeleteProductHandler(repository);
        var publicId = Guid.NewGuid();

        var result = await handler.HandleAsync(
            new DeleteProductCommand(publicId),
            TestContext.Current.CancellationToken);

        Assert.True(result.Deleted);
        Assert.Equal(publicId, repository.DeletedPublicId);
    }

    [Fact]
    public async Task HandleAsync_MissingProduct_ReturnsNotDeleted()
    {
        var repository = new ProductCommandRepositoryStub { DeleteResult = false };
        var handler = new DeleteProductHandler(repository);

        var result = await handler.HandleAsync(
            new DeleteProductCommand(Guid.NewGuid()),
            TestContext.Current.CancellationToken);

        Assert.False(result.Deleted);
    }
}
