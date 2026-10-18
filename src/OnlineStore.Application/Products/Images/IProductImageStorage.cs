namespace OnlineStore.Application.Products.Images;

public interface IProductImageStorage
{
    Task<ProductImageUpload> CreateUploadAsync(
        string blobName,
        string contentType,
        CancellationToken ct);

    Task<ProductImageBlobProperties?> GetPropertiesAsync(
        string blobName,
        CancellationToken ct);

    string GetPublicUrl(string blobName);

    Task DeleteAsync(
        string blobName,
        CancellationToken ct);
}
