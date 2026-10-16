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
    public DateTimeOffset OrderDate { get; private set; }
    public DateTimeOffset UpdatedAt { get; private set; }
    public DateTimeOffset? ShippingDate { get; private set; }
    public TimeSpan? ProcessingDuration => ShippingDate - OrderDate;
    public decimal TotalAmount { get; private set; }
    public string? RejectionReason { get; private set; }
    public IReadOnlyCollection<OrderItem> Items => items;

    private Order() { }

    public Order(
        Guid publicId,
        Guid customerPublicId,
        CustomerDetails customer,
        DeliveryAddress deliveryAddress,
        DateTimeOffset orderDate,
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
        OrderDate = orderDate;
        UpdatedAt = orderDate;
        Status = OrderStatus.Pending;
        items.AddRange(materializedItems);
        TotalAmount = items.Sum(item => item.LineTotal);
    }

    public static Order Rejected(
        Guid publicId,
        Guid customerPublicId,
        CustomerDetails customer,
        DeliveryAddress deliveryAddress,
        DateTimeOffset orderDate,
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
            OrderDate = orderDate,
            UpdatedAt = orderDate,
            Status = OrderStatus.Rejected,
            RejectionReason = reason.Trim()
        };
    }

    public void Process(DateTimeOffset processedAt)
    {
        EnsureStatus(OrderStatus.Pending, "processed");
        Status = OrderStatus.Processing;
        UpdatedAt = processedAt;
    }

    public void Ship(DateTimeOffset shippingDate)
    {
        EnsureStatus(OrderStatus.Processing, "shipped");

        if (shippingDate < OrderDate)
        {
            throw new ArgumentOutOfRangeException(
                nameof(shippingDate),
                "The shipping date cannot be before the order date.");
        }

        ShippingDate = shippingDate;
        Status = OrderStatus.Shipped;
        UpdatedAt = shippingDate;
    }

    public void Cancel(DateTimeOffset cancelledAt)
    {
        if (Status is not OrderStatus.Pending and not OrderStatus.Processing)
        {
            throw new InvalidOperationException(
                $"An order with status {Status} cannot be cancelled.");
        }

        Status = OrderStatus.Cancelled;
        UpdatedAt = cancelledAt;
    }

    private void EnsureStatus(
        OrderStatus requiredStatus,
        string action)
    {
        if (Status != requiredStatus)
        {
            throw new InvalidOperationException(
                $"An order with status {Status} cannot be {action}.");
        }
    }

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
