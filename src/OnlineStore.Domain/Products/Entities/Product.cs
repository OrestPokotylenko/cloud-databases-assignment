using OnlineStore.Domain.Products;

namespace OnlineStore.Domain.Products.Entities;

public class Product
{
    public int Id { get; private set; }
    public Guid PublicId { get; private set; }
    public string Name { get; private set; } = null!;
    public string? Description { get; private set; }
    public decimal Price { get; private set; }
    public int StockQuantity { get; private set; }
    public DateTimeOffset CreatedAt { get; private set; }

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
}
