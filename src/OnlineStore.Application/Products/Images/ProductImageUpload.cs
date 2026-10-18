namespace OnlineStore.Application.Products.Images;

public record ProductImageUpload(
    string UploadUrl,
    DateTimeOffset ExpiresAt);
