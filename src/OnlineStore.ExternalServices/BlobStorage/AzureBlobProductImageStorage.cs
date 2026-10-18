using Azure;
using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using Azure.Storage.Sas;

using Microsoft.Extensions.Options;

using OnlineStore.Application.Products.Images;
using OnlineStore.ExternalServices.Options;

namespace OnlineStore.ExternalServices.BlobStorage;

public class AzureBlobProductImageStorage(
    BlobContainerClient container,
    IOptions<ProductImageStorageOptions> options,
    TimeProvider timeProvider)
    : IProductImageStorage
{
    private readonly SemaphoreSlim initializationLock = new(1, 1);
    private volatile bool initialized;

    public async Task<ProductImageUpload> CreateUploadAsync(
        string blobName,
        string contentType,
        CancellationToken ct)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(blobName);
        ArgumentException.ThrowIfNullOrWhiteSpace(contentType);

        await EnsureContainerExistsAsync(ct);

        var blob = container.GetBlobClient(blobName);
        if (!blob.CanGenerateSasUri)
        {
            throw new InvalidOperationException(
                "The configured Blob Storage credentials cannot generate an upload SAS URL.");
        }

        var now = timeProvider.GetUtcNow();
        var expiresAt = now.AddMinutes(options.Value.UploadUrlLifetimeMinutes);
        var sas = new BlobSasBuilder
        {
            BlobContainerName = container.Name,
            BlobName = blobName,
            Resource = "b",
            StartsOn = now.AddMinutes(-1),
            ExpiresOn = expiresAt,
            Protocol = blob.Uri.Scheme == Uri.UriSchemeHttps
                ? SasProtocol.Https
                : SasProtocol.HttpsAndHttp
        };
        sas.SetPermissions(BlobSasPermissions.Create | BlobSasPermissions.Write);

        return new ProductImageUpload(
            blob.GenerateSasUri(sas).AbsoluteUri,
            expiresAt);
    }

    public async Task<ProductImageBlobProperties?> GetPropertiesAsync(
        string blobName,
        CancellationToken ct)
    {
        await EnsureContainerExistsAsync(ct);

        try
        {
            var response = await container
                .GetBlobClient(blobName)
                .GetPropertiesAsync(cancellationToken: ct);

            return new ProductImageBlobProperties(
                response.Value.ContentType ?? string.Empty,
                response.Value.ContentLength);
        }
        catch (RequestFailedException exception) when (exception.Status == 404)
        {
            return null;
        }
    }

    public string GetPublicUrl(string blobName) =>
        container.GetBlobClient(blobName).Uri.AbsoluteUri;

    public async Task DeleteAsync(
        string blobName,
        CancellationToken ct)
    {
        await EnsureContainerExistsAsync(ct);
        await container
            .GetBlobClient(blobName)
            .DeleteIfExistsAsync(
                DeleteSnapshotsOption.IncludeSnapshots,
                cancellationToken: ct);
    }

    private async Task EnsureContainerExistsAsync(CancellationToken ct)
    {
        if (initialized)
        {
            return;
        }

        await initializationLock.WaitAsync(ct);
        try
        {
            if (!initialized)
            {
                await container.CreateIfNotExistsAsync(
                    PublicAccessType.Blob,
                    cancellationToken: ct);
                initialized = true;
            }
        }
        finally
        {
            initializationLock.Release();
        }
    }
}
