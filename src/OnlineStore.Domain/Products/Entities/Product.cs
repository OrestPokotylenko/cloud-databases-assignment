using OnlineStore.Domain.Products;

namespace OnlineStore.Domain.Products.Entities;

public class Product
{
    private readonly List<ProductImage> images = [];
    private string? name;

    public int Id { get; private set; }
    public Guid PublicId { get; private set; }
    public string Name
    {
        get => name ?? throw new InvalidOperationException("The product name has not been initialized.");
        private set => name = value;
    }
    public string? Description { get; private set; }
    public decimal Price { get; private set; }
    public int StockQuantity { get; private set; }
    public DateTimeOffset CreatedAt { get; private set; }
    public IReadOnlyCollection<ProductImage> Images => images;

    private Product() { }

    public Product(
        string name,
        string? description,
        decimal price,
        int stockQuantity)
    {
        Update(new ProductDetails(name, description, price, stockQuantity));
    }

    public void Update(ProductDetails details)
    {
        ArgumentNullException.ThrowIfNull(details);

        Name = details.Name;
        Description = details.Description;
        Price = details.Price;
        StockQuantity = details.StockQuantity;
    }

    public void DecreaseStock(int quantity)
    {
        if (quantity < 1 || quantity > StockQuantity)
        {
            throw new ArgumentOutOfRangeException(nameof(quantity));
        }

        StockQuantity -= quantity;
    }

    public void IncreaseStock(int quantity)
    {
        if (quantity < 1)
        {
            throw new ArgumentOutOfRangeException(nameof(quantity));
        }

        StockQuantity = checked(StockQuantity + quantity);
    }

    public ProductImage AddImage(
        Guid publicId,
        string blobName,
        string contentType,
        long sizeBytes)
    {
        if (images.Any(image => image.PublicId == publicId))
        {
            throw new InvalidOperationException("The image already belongs to this product.");
        }

        var sortOrder = images.Count == 0
            ? 0
            : images.Max(image => image.SortOrder) + 1;
        var image = new ProductImage(
            publicId,
            blobName,
            contentType,
            sizeBytes,
            images.Count == 0,
            sortOrder);

        images.Add(image);
        return image;
    }

    public void RemoveImage(Guid imagePublicId)
    {
        var image = images.SingleOrDefault(image => image.PublicId == imagePublicId)
            ?? throw new InvalidOperationException("The image does not belong to this product.");
        var wasPrimary = image.IsPrimary;

        images.Remove(image);

        var orderedImages = images
            .OrderBy(remainingImage => remainingImage.SortOrder)
            .ToList();
        for (var index = 0; index < orderedImages.Count; index++)
        {
            orderedImages[index].SetSortOrder(index);
        }

        if (wasPrimary && orderedImages.Count > 0)
        {
            orderedImages[0].SetPrimary(true);
        }
    }

    public void SetPrimaryImage(Guid imagePublicId)
    {
        var primaryImage = images.SingleOrDefault(image => image.PublicId == imagePublicId)
            ?? throw new InvalidOperationException("The image does not belong to this product.");

        foreach (var image in images)
        {
            image.SetPrimary(image == primaryImage);
        }
    }
}
