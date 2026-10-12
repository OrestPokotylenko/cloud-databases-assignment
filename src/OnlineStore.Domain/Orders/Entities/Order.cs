namespace OnlineStore.Domain.Orders.Entities;

public class Order
{
    private readonly List<OrderItem> items = [];
    private CustomerDetails? customer;
    private DeliveryAddress? deliveryAddress;

    public int Id { get; private set; }
    public Guid PublicId { get; private set; }
    public Guid CustomerPublicId { get; private set; }
    public CustomerDetails Customer
    {
        get => customer ?? throw new InvalidOperationException("The customer has not been initialized.");
        private set => customer = value;
    }

    public DeliveryAddress DeliveryAddress
    {
        get => deliveryAddress ?? throw new InvalidOperationException("The delivery address has not been initialized.");
        private set => deliveryAddress = value;
    }
    public OrderStatus Status { get; private set; }
    public DateTimeOffset OrderedAt { get; private set; }
    public DateTimeOffset UpdatedAt { get; private set; }
    public DateTimeOffset? ShippedAt { get; private set; }
    public decimal TotalAmount { get; private set; }
    public string? RejectionReason { get; private set; }
    public IReadOnlyCollection<OrderItem> Items => items;

    private Order() { }

    public Order(
        Guid publicId,
        Guid customerPublicId,
        CustomerDetails customer,
        DeliveryAddress deliveryAddress,
        DateTimeOffset orderedAt,
        IEnumerable<OrderItem> orderItems)
    {
        ValidateIdentity(publicId, customerPublicId);
        ArgumentNullException.ThrowIfNull(customer);
        ArgumentNullException.ThrowIfNull(deliveryAddress);
        ArgumentNullException.ThrowIfNull(orderItems);

        var materializedItems = orderItems.ToList();
        if (materializedItems.Count == 0)
        {
            throw new ArgumentException("An order must contain at least one item.", nameof(orderItems));
        }

        PublicId = publicId;
        CustomerPublicId = customerPublicId;
        Customer = customer;
        DeliveryAddress = deliveryAddress;
        OrderedAt = orderedAt;
        UpdatedAt = orderedAt;
        Status = OrderStatus.Pending;
        items.AddRange(materializedItems);
        TotalAmount = items.Sum(item => item.LineTotal);
    }

    public static Order Rejected(
        Guid publicId,
        Guid customerPublicId,
        CustomerDetails customer,
        DeliveryAddress deliveryAddress,
        DateTimeOffset orderedAt,
        string reason)
    {
        ValidateIdentity(publicId, customerPublicId);
        ArgumentNullException.ThrowIfNull(customer);
        ArgumentNullException.ThrowIfNull(deliveryAddress);
        ArgumentException.ThrowIfNullOrWhiteSpace(reason);

        return new Order
        {
            PublicId = publicId,
            CustomerPublicId = customerPublicId,
            Customer = customer,
            DeliveryAddress = deliveryAddress,
            OrderedAt = orderedAt,
            UpdatedAt = orderedAt,
            Status = OrderStatus.Rejected,
            RejectionReason = reason.Trim()
        };
    }

    public bool UpdateStatus(
        OrderStatus status,
        DateTimeOffset updatedAt)
    {
        if (status == Status)
        {
            return false;
        }

        if (!CanTransitionTo(status))
        {
            throw new InvalidOperationException(
                $"Order status cannot change from {Status} to {status}.");
        }

        Status = status;
        UpdatedAt = updatedAt;
        if (status == OrderStatus.Shipped)
        {
            ShippedAt = updatedAt;
        }

        return true;
    }

    public bool CanTransitionTo(OrderStatus status) =>
        status == Status ||
        Status == OrderStatus.Pending && status is OrderStatus.Processing or OrderStatus.Cancelled ||
        Status == OrderStatus.Processing && status is OrderStatus.Shipped or OrderStatus.Cancelled;

    private static void ValidateIdentity(
        Guid publicId,
        Guid customerPublicId)
    {
        if (publicId == Guid.Empty)
        {
            throw new ArgumentOutOfRangeException(nameof(publicId));
        }

        if (customerPublicId == Guid.Empty)
        {
            throw new ArgumentOutOfRangeException(nameof(customerPublicId));
        }
    }
}
