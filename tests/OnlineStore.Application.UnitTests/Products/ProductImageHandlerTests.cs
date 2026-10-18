using Microsoft.Extensions.Options;

using OnlineStore.Application.Products.ConfirmProductImageUpload;
using OnlineStore.Application.Products.DeleteProductImage;
using OnlineStore.Application.Products.Images;
using OnlineStore.Application.Products.RequestProductImageUpload;

using Xunit;

namespace OnlineStore.Application.UnitTests.Products;

public class ProductImageHandlerTests
{
    [Fact]
    public async Task RequestUpload_ValidImage_ReturnsBlobScopedUpload()
    {
        var productPublicId = Guid.NewGuid();
        var repository = new ProductImageRepositoryStub();
        var storage = new ProductImageStorageStub();
        var handler = new RequestProductImageUploadHandler(
            repository,
            storage,
            CreateOptions());

        var result = await handler.HandleAsync(
            new RequestProductImageUploadCommand(
                productPublicId,
                " Image/JPEG ",
                500),
            TestContext.Current.CancellationToken);

        Assert.True(result.ProductExists);
        var imagePublicId = Assert.IsType<Guid>(result.ImagePublicId);
        Assert.NotEqual(Guid.Empty, imagePublicId);
        Assert.Equal(
            $"{productPublicId:D}/{imagePublicId:D}",
            storage.RequestedBlobName);
        Assert.Equal("image/jpeg", storage.RequestedContentType);
        Assert.Equal(storage.Upload.UploadUrl, result.UploadUrl);
    }

    [Fact]
    public async Task RequestUpload_UnsupportedContentType_RejectsRequest()
    {
        var handler = new RequestProductImageUploadHandler(
            new ProductImageRepositoryStub(),
            new ProductImageStorageStub(),
            CreateOptions());

        await Assert.ThrowsAsync<ArgumentException>(() =>
            handler.HandleAsync(
                new RequestProductImageUploadCommand(
                    Guid.NewGuid(),
                    "image/gif",
                    500),
                TestContext.Current.CancellationToken));
    }

    [Fact]
    public async Task ConfirmUpload_ExistingValidBlob_PersistsMetadata()
    {
        var productPublicId = Guid.NewGuid();
        var imagePublicId = Guid.NewGuid();
        var repository = new ProductImageRepositoryStub();
        var storage = new ProductImageStorageStub
        {
            Properties = new ProductImageBlobProperties("image/webp", 750)
        };
        var handler = new ConfirmProductImageUploadHandler(
            repository,
            repository,
            storage,
            CreateOptions());

        var result = await handler.HandleAsync(
            new ConfirmProductImageUploadCommand(
                productPublicId,
                imagePublicId),
            TestContext.Current.CancellationToken);

        Assert.True(result.ProductExists);
        Assert.NotNull(result.Image);
        Assert.Equal(imagePublicId, repository.AddedImage?.PublicId);
        Assert.Equal("image/webp", repository.AddedImage?.ContentType);
        Assert.Equal(750, repository.AddedImage?.SizeBytes);
        Assert.Empty(storage.DeletedBlobNames);
    }

    [Fact]
    public async Task ConfirmUpload_InvalidBlob_DeletesBlobAndRejectsRequest()
    {
        var productPublicId = Guid.NewGuid();
        var imagePublicId = Guid.NewGuid();
        var repository = new ProductImageRepositoryStub();
        var storage = new ProductImageStorageStub
        {
            Properties = new ProductImageBlobProperties("image/gif", 750)
        };
        var handler = new ConfirmProductImageUploadHandler(
            repository,
            repository,
            storage,
            CreateOptions());

        await Assert.ThrowsAsync<ArgumentException>(() =>
            handler.HandleAsync(
                new ConfirmProductImageUploadCommand(
                    productPublicId,
                    imagePublicId),
                TestContext.Current.CancellationToken));

        Assert.Equal(
            [$"{productPublicId:D}/{imagePublicId:D}"],
            storage.DeletedBlobNames);
        Assert.Null(repository.AddedImage);
    }

    [Fact]
    public async Task DeleteImage_OwnedImage_DeletesBlobAndMetadata()
    {
        var productPublicId = Guid.NewGuid();
        var imagePublicId = Guid.NewGuid();
        var blobName = $"{productPublicId:D}/{imagePublicId:D}";
        var repository = new ProductImageRepositoryStub
        {
            Image = new ProductImageData(
                imagePublicId,
                blobName,
                "image/jpeg",
                100,
                true,
                0)
        };
        var storage = new ProductImageStorageStub();
        var handler = new DeleteProductImageHandler(
            repository,
            repository,
            storage);

        var result = await handler.HandleAsync(
            new DeleteProductImageCommand(
                productPublicId,
                imagePublicId),
            TestContext.Current.CancellationToken);

        Assert.True(result.Deleted);
        Assert.Equal([blobName], storage.DeletedBlobNames);
        Assert.Equal(imagePublicId, repository.RequestedImagePublicId);
    }

    private static IOptions<ProductImageValidationOptions> CreateOptions() =>
        Options.Create(new ProductImageValidationOptions
        {
            MaximumSizeBytes = 1_000,
            SupportedContentTypes =
            [
                "image/jpeg",
                "image/png",
                "image/webp"
            ]
        });
}
