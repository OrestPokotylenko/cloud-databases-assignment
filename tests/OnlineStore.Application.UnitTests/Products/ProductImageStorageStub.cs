using OnlineStore.Application.Products.Images;

namespace OnlineStore.Application.UnitTests.Products;

internal class ProductImageStorageStub : IProductImageStorage
{
    public ProductImageUpload Upload { get; set; } = new(
        "https://storage.example/upload?sas=token",
        DateTimeOffset.UtcNow.AddMinutes(10));
    public ProductImageBlobProperties? Properties { get; set; }
    public string? RequestedBlobName { get; private set; }
    public string? RequestedContentType { get; private set; }
    public List<string> DeletedBlobNames { get; } = [];
    public CancellationToken ReceivedCancellationToken { get; private set; }

    public Task<ProductImageUpload> CreateUploadAsync(
        string blobName,
        string contentType,
        CancellationToken ct)
    {
        RequestedBlobName = blobName;
        RequestedContentType = contentType;
        ReceivedCancellationToken = ct;
        return Task.FromResult(Upload);
    }

    public Task<ProductImageBlobProperties?> GetPropertiesAsync(
        string blobName,
        CancellationToken ct)
    {
        RequestedBlobName = blobName;
        ReceivedCancellationToken = ct;
        return Task.FromResult(Properties);
    }

    public string GetPublicUrl(string blobName) =>
        $"https://storage.example/product-images/{blobName}";

    public Task DeleteAsync(
        string blobName,
        CancellationToken ct)
    {
        DeletedBlobNames.Add(blobName);
        ReceivedCancellationToken = ct;
        return Task.CompletedTask;
    }
}
