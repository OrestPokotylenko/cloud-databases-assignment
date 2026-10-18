namespace OnlineStore.Application.Products.RequestProductImageUpload;

public record RequestProductImageUploadResult(
    bool ProductExists,
    Guid? ImagePublicId,
    string? UploadUrl,
    DateTimeOffset? ExpiresAt);
