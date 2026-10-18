using OnlineStore.Domain.Products.Entities;

using Xunit;

namespace OnlineStore.Application.UnitTests.Products;

public class ProductImageTests
{
    [Fact]
    public void AddImage_FirstImage_BecomesPrimary()
    {
        var product = CreateProduct();

        var image = product.AddImage(
            Guid.NewGuid(),
            "product/image",
            "image/jpeg",
            100);

        Assert.True(image.IsPrimary);
        Assert.Equal(0, image.SortOrder);
    }

    [Fact]
    public void SetPrimaryImage_ChangesTheOnlyPrimaryImage()
    {
        var product = CreateProduct();
        var first = product.AddImage(
            Guid.NewGuid(),
            "product/first",
            "image/jpeg",
            100);
        var second = product.AddImage(
            Guid.NewGuid(),
            "product/second",
            "image/png",
            200);

        product.SetPrimaryImage(second.PublicId);

        Assert.False(first.IsPrimary);
        Assert.True(second.IsPrimary);
        Assert.Single(product.Images, image => image.IsPrimary);
    }

    [Fact]
    public void RemoveImage_PrimaryImage_PromotesFirstRemainingImageAndCompactsOrder()
    {
        var product = CreateProduct();
        var first = product.AddImage(
            Guid.NewGuid(),
            "product/first",
            "image/jpeg",
            100);
        var second = product.AddImage(
            Guid.NewGuid(),
            "product/second",
            "image/png",
            200);
        var third = product.AddImage(
            Guid.NewGuid(),
            "product/third",
            "image/webp",
            300);

        product.RemoveImage(first.PublicId);

        Assert.True(second.IsPrimary);
        Assert.Equal(0, second.SortOrder);
        Assert.Equal(1, third.SortOrder);
        Assert.Single(product.Images, image => image.IsPrimary);
    }

    private static Product CreateProduct() => new(
        "Product",
        null,
        10m,
        5);
}
