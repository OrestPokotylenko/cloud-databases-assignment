namespace OnlineStore.Domain.Products.Entities;

public class ProductImage
{
    private string? blobName;
    private string? contentType;

    public int Id { get; private set; }
    public Guid PublicId { get; private set; }
    public int ProductId { get; private set; }
    public string BlobName
    {
        get => blobName ?? throw new InvalidOperationException("The blob name has not been initialized.");
        private set => blobName = value;
    }
    public string ContentType
    {
        get => contentType ?? throw new InvalidOperationException("The content type has not been initialized.");
        private set => contentType = value;
    }
    public long SizeBytes { get; private set; }
    public bool IsPrimary { get; private set; }
    public int SortOrder { get; private set; }

    private ProductImage() { }

    internal ProductImage(
        Guid publicId,
        string blobName,
        string contentType,
        long sizeBytes,
        bool isPrimary,
        int sortOrder)
    {
        ArgumentOutOfRangeException.ThrowIfEqual(publicId, Guid.Empty);
        ArgumentException.ThrowIfNullOrWhiteSpace(blobName);
        ArgumentException.ThrowIfNullOrWhiteSpace(contentType);
        ArgumentOutOfRangeException.ThrowIfNegativeOrZero(sizeBytes);
        ArgumentOutOfRangeException.ThrowIfNegative(sortOrder);

        PublicId = publicId;
        BlobName = blobName.Trim();
        ContentType = contentType.Trim().ToLowerInvariant();
        SizeBytes = sizeBytes;
        IsPrimary = isPrimary;
        SortOrder = sortOrder;
    }

    internal void SetPrimary(bool isPrimary)
    {
        IsPrimary = isPrimary;
    }

    internal void SetSortOrder(int sortOrder)
    {
        ArgumentOutOfRangeException.ThrowIfNegative(sortOrder);
        SortOrder = sortOrder;
    }
}
