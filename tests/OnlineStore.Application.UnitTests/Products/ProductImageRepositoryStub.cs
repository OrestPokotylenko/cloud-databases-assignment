using OnlineStore.Application.Products.Images;

namespace OnlineStore.Application.UnitTests.Products;

internal class ProductImageRepositoryStub
    : IProductImageCommandRepository, IProductImageQueryRepository
{
    public bool ProductExists { get; set; } = true;
    public ProductImageData? Image { get; set; }
    public IReadOnlyList<ProductImageData> Images { get; set; } = [];
    public bool DeleteResult { get; set; } = true;
    public bool SetPrimaryResult { get; set; } = true;
    public ProductImageData? AddedImage { get; private set; }
    public Guid? RequestedProductPublicId { get; private set; }
    public Guid? RequestedImagePublicId { get; private set; }
    public CancellationToken ReceivedCancellationToken { get; private set; }

    public Task<bool> ProductExistsAsync(
        Guid productPublicId,
        CancellationToken ct)
    {
        RequestedProductPublicId = productPublicId;
        ReceivedCancellationToken = ct;
        return Task.FromResult(ProductExists);
    }

    public Task<ProductImageData?> GetAsync(
        Guid productPublicId,
        Guid imagePublicId,
        CancellationToken ct)
    {
        RequestedProductPublicId = productPublicId;
        RequestedImagePublicId = imagePublicId;
        ReceivedCancellationToken = ct;
        return Task.FromResult(Image);
    }

    public Task<ProductImageCollectionData> GetAllAsync(
        Guid productPublicId,
        CancellationToken ct)
    {
        RequestedProductPublicId = productPublicId;
        ReceivedCancellationToken = ct;
        return Task.FromResult(new ProductImageCollectionData(
            ProductExists,
            Images));
    }

    public Task<ProductImageData?> AddAsync(
        Guid productPublicId,
        Guid imagePublicId,
        string blobName,
        string contentType,
        long sizeBytes,
        CancellationToken ct)
    {
        RequestedProductPublicId = productPublicId;
        RequestedImagePublicId = imagePublicId;
        ReceivedCancellationToken = ct;
        AddedImage = ProductExists
            ? new ProductImageData(
                imagePublicId,
                blobName,
                contentType,
                sizeBytes,
                Images.Count == 0,
                Images.Count)
            : null;

        return Task.FromResult(AddedImage);
    }

    public Task<bool> DeleteAsync(
        Guid productPublicId,
        Guid imagePublicId,
        CancellationToken ct)
    {
        RequestedProductPublicId = productPublicId;
        RequestedImagePublicId = imagePublicId;
        ReceivedCancellationToken = ct;
        return Task.FromResult(DeleteResult);
    }

    public Task<bool> SetPrimaryAsync(
        Guid productPublicId,
        Guid imagePublicId,
        CancellationToken ct)
    {
        RequestedProductPublicId = productPublicId;
        RequestedImagePublicId = imagePublicId;
        ReceivedCancellationToken = ct;
        return Task.FromResult(SetPrimaryResult);
    }
}
