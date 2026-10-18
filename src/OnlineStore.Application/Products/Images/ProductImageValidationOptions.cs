namespace OnlineStore.Application.Products.Images;

public sealed class ProductImageValidationOptions
{
    public const string SectionName = "ProductImages";

    public long MaximumSizeBytes { get; init; }
    public required string[] SupportedContentTypes { get; init; }
}
