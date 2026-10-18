namespace OnlineStore.API.Products;

public record ProductImageUploadResponse(
    Guid ImageId,
    string UploadUrl,
    DateTimeOffset ExpiresAt);
