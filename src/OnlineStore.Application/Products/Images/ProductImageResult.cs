namespace OnlineStore.Application.Products.Images;

public record ProductImageResult(
    Guid PublicId,
    string Url,
    string ContentType,
    long SizeBytes,
    bool IsPrimary,
    int SortOrder);
