using OnlineStore.Domain.Products;

namespace OnlineStore.Domain.Products.Entities;

public class Product
{
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
}
