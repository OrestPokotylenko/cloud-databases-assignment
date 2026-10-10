namespace OnlineStore.Domain.Products;

public sealed record ProductDetails
{
    public ProductDetails(
        string name,
        string? description,
        decimal price,
        int stockQuantity)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(name);

        var normalizedName = name.Trim();
        var normalizedDescription = string.IsNullOrWhiteSpace(description)
            ? null
            : description.Trim();

        if (normalizedName.Length > 200)
        {
            throw new ArgumentException(
                "The product name cannot exceed 200 characters.",
                nameof(name));
        }

        if (normalizedDescription?.Length > 2000)
        {
            throw new ArgumentException(
                "The product description cannot exceed 2000 characters.",
                nameof(description));
        }

        if (price < 0)
        {
            throw new ArgumentOutOfRangeException(
                nameof(price),
                "The product price cannot be negative.");
        }

        if (stockQuantity < 0)
        {
            throw new ArgumentOutOfRangeException(
                nameof(stockQuantity),
                "The stock quantity cannot be negative.");
        }

        Name = normalizedName;
        Description = normalizedDescription;
        Price = price;
        StockQuantity = stockQuantity;
    }

    public string Name { get; }
    public string? Description { get; }
    public decimal Price { get; }
    public int StockQuantity { get; }
}
