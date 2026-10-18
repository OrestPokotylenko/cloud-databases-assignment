namespace OnlineStore.Application.Products.Images;

public record ProductImageData(
    Guid PublicId,
    string BlobName,
    string ContentType,
    long SizeBytes,
    bool IsPrimary,
    int SortOrder);
