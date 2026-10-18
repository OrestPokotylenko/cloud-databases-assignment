namespace OnlineStore.ExternalServices.Options;

public sealed class ProductReviewStorageOptions
{
    public const string SectionName = "ProductReviews";

    public required string ConnectionString { get; init; }
    public required string TableName { get; init; }
}
