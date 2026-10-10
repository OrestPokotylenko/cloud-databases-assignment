using OnlineStore.Application.Products.CreateProduct;

using Xunit;

namespace OnlineStore.Application.UnitTests.Products;

public sealed class CreateProductHandlerTests
{
    [Fact]
    public async Task HandleAsync_ValidCommand_AddsNormalizedProduct()
    {
        var repository = new ProductCommandRepositoryStub();
        var handler = new CreateProductHandler(repository);
        using var cancellationSource = new CancellationTokenSource();

        var result = await handler.HandleAsync(
            new CreateProductCommand("  Widget  ", "  Useful  ", 12.50m, 7),
            cancellationSource.Token);

        var product = Assert.IsType<Domain.Products.Entities.Product>(
            repository.AddedProduct);
        Assert.Equal(repository.AddedPublicId, result.PublicId);
        Assert.Equal(Guid.Empty, product.PublicId);
        Assert.Equal("Widget", product.Name);
        Assert.Equal("Useful", product.Description);
        Assert.Equal(12.50m, product.Price);
        Assert.Equal(7, product.StockQuantity);
        Assert.Equal(cancellationSource.Token, repository.ReceivedCancellationToken);
    }

    [Fact]
    public async Task HandleAsync_NegativePrice_RejectsCommandWithoutWriting()
    {
        var repository = new ProductCommandRepositoryStub();
        var handler = new CreateProductHandler(repository);

        await Assert.ThrowsAsync<ArgumentOutOfRangeException>(() =>
            handler.HandleAsync(
                new CreateProductCommand("Widget", null, -0.01m, 1),
                TestContext.Current.CancellationToken));

        Assert.Null(repository.AddedProduct);
    }
}
