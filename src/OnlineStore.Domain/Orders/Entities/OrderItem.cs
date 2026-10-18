namespace OnlineStore.Domain.Orders.Entities;

public class OrderItem
{
    private string? productName;

    public int Id { get; private set; }
    public int OrderId { get; private set; }
    public Guid ProductPublicId { get; private set; }
    public string ProductName
    {
        get => productName ?? throw new InvalidOperationException("The product name has not been initialized.");
        private set => productName = value;
    }
    public decimal UnitPrice { get; private set; }
    public int Quantity { get; private set; }
    public decimal LineTotal => UnitPrice * Quantity;

    private OrderItem() { }

    public OrderItem(
        Guid productPublicId,
        string productName,
        decimal unitPrice,
        int quantity)
    {
        if (productPublicId == Guid.Empty)
        {
            throw new ArgumentOutOfRangeException(nameof(productPublicId));
        }

        ArgumentException.ThrowIfNullOrWhiteSpace(productName);

        if (unitPrice < 0)
        {
            throw new ArgumentOutOfRangeException(nameof(unitPrice));
        }

        if (quantity < 1)
        {
            throw new ArgumentOutOfRangeException(nameof(quantity));
        }

        ProductPublicId = productPublicId;
        ProductName = productName.Trim();
        UnitPrice = unitPrice;
        Quantity = quantity;
    }
}
