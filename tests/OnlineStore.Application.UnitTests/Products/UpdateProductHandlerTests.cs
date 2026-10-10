using OnlineStore.Application.Products.UpdateProduct;

using Xunit;

namespace OnlineStore.Application.UnitTests.Products;

public sealed class UpdateProductHandlerTests
{
    [Fact]
    public async Task HandleAsync_ValidCommand_ForwardsNormalizedUpdate()
    {
        var repository = new ProductCommandRepositoryStub();
        var handler = new UpdateProductHandler(repository);
        var publicId = Guid.NewGuid();

        var result = await handler.HandleAsync(
            new UpdateProductCommand(publicId, "  Widget  ", "   ", 10m, 3),
            TestContext.Current.CancellationToken);

        Assert.True(result.Updated);
        Assert.NotNull(repository.ReceivedUpdate);
        Assert.Equal(publicId, repository.ReceivedUpdate.PublicId);
        Assert.Equal("Widget", repository.ReceivedUpdate.Details.Name);
        Assert.Null(repository.ReceivedUpdate.Details.Description);
    }

    [Fact]
    public async Task HandleAsync_EmptyPublicId_RejectsCommandWithoutWriting()
    {
        var repository = new ProductCommandRepositoryStub();
        var handler = new UpdateProductHandler(repository);

        await Assert.ThrowsAsync<ArgumentOutOfRangeException>(() =>
            handler.HandleAsync(
                new UpdateProductCommand(Guid.Empty, "Widget", null, 10m, 3),
                TestContext.Current.CancellationToken));

        Assert.Null(repository.ReceivedUpdate);
    }
}
