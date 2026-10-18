namespace OnlineStore.ExternalServices.Options;

public sealed class ProductImageStorageOptions
{
    public const string SectionName = "ProductImages";

    public required string ConnectionString { get; init; }
    public required string ContainerName { get; init; }
    public int UploadUrlLifetimeMinutes { get; init; }
}
