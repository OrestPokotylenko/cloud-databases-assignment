using OnlineStore.Domain.Orders;
using OnlineStore.Domain.Orders.Entities;

using Xunit;

namespace OnlineStore.Application.UnitTests.Orders;

public class OrderTests
{
    [Fact]
    public void Ship_ProcessingOrder_SetsShippingDateAndProcessingDuration()
    {
        var orderDate = DateTimeOffset.UtcNow;
        var shippingDate = orderDate.AddHours(2);
        var order = CreateOrder(orderDate);

        order.Process(orderDate.AddMinutes(1));
        order.Ship(shippingDate);

        Assert.Equal(OrderStatus.Shipped, order.Status);
        Assert.Equal(shippingDate, order.ShippingDate);
        Assert.Equal(TimeSpan.FromHours(2), order.ProcessingDuration);
    }

    [Fact]
    public void Process_PendingOrder_ChangesStatusToProcessing()
    {
        var order = CreateOrder(DateTimeOffset.UtcNow);

        order.Process(DateTimeOffset.UtcNow);

        Assert.Equal(OrderStatus.Processing, order.Status);
    }

    [Fact]
    public void Cancel_PendingOrder_ChangesStatusToCancelled()
    {
        var order = CreateOrder(DateTimeOffset.UtcNow);

        order.Cancel(DateTimeOffset.UtcNow);

        Assert.Equal(OrderStatus.Cancelled, order.Status);
    }

    [Fact]
    public void Cancel_ProcessingOrder_ChangesStatusToCancelled()
    {
        var order = CreateOrder(DateTimeOffset.UtcNow);
        order.Process(DateTimeOffset.UtcNow);

        order.Cancel(DateTimeOffset.UtcNow);

        Assert.Equal(OrderStatus.Cancelled, order.Status);
    }

    [Fact]
    public void Ship_PendingOrder_Throws()
    {
        var order = CreateOrder(DateTimeOffset.UtcNow);

        Assert.Throws<InvalidOperationException>(() =>
            order.Ship(DateTimeOffset.UtcNow));
    }

    [Theory]
    [InlineData(OrderStatus.Shipped)]
    [InlineData(OrderStatus.Cancelled)]
    public void TerminalOrder_CannotBeChanged(OrderStatus terminalStatus)
    {
        var order = CreateOrder(DateTimeOffset.UtcNow);
        if (terminalStatus == OrderStatus.Shipped)
        {
            order.Process(DateTimeOffset.UtcNow);
            order.Ship(DateTimeOffset.UtcNow);
        }
        else
        {
            order.Cancel(DateTimeOffset.UtcNow);
        }

        Assert.Throws<InvalidOperationException>(() =>
            order.Process(DateTimeOffset.UtcNow));
        Assert.Throws<InvalidOperationException>(() =>
            order.Ship(DateTimeOffset.UtcNow));
        Assert.Throws<InvalidOperationException>(() =>
            order.Cancel(DateTimeOffset.UtcNow));
    }

    private static Order CreateOrder(DateTimeOffset orderDate) => new(
        Guid.NewGuid(),
        Guid.NewGuid(),
        new CustomerDetails("Customer", "customer@example.com", null),
        new DeliveryAddress("Street", "1", null, "1000 AA", "Amsterdam", "NL"),
        orderDate,
        [new OrderItem(Guid.NewGuid(), "Product", 10m, 2)]);
}
