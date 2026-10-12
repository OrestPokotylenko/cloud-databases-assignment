using OnlineStore.Domain.Orders;
using OnlineStore.Domain.Orders.Entities;

using Xunit;

namespace OnlineStore.Application.UnitTests.Orders;

public class OrderTests
{
    [Fact]
    public void UpdateStatus_ValidProgression_SetsShippingDate()
    {
        var orderedAt = DateTimeOffset.UtcNow;
        var shippedAt = orderedAt.AddHours(2);
        var order = CreateOrder(orderedAt);

        order.UpdateStatus(OrderStatus.Processing, orderedAt.AddMinutes(1));
        order.UpdateStatus(OrderStatus.Shipped, shippedAt);

        Assert.Equal(OrderStatus.Shipped, order.Status);
        Assert.Equal(shippedAt, order.ShippedAt);
    }

    [Fact]
    public void UpdateStatus_BackwardTransition_Throws()
    {
        var order = CreateOrder(DateTimeOffset.UtcNow);
        order.UpdateStatus(OrderStatus.Processing, DateTimeOffset.UtcNow);

        Assert.Throws<InvalidOperationException>(() =>
            order.UpdateStatus(OrderStatus.Pending, DateTimeOffset.UtcNow));
    }

    private static Order CreateOrder(DateTimeOffset orderedAt) => new(
        Guid.NewGuid(),
        Guid.NewGuid(),
        new CustomerDetails("Customer", "customer@example.com", null),
        new DeliveryAddress("Street", "1", null, "1000 AA", "Amsterdam", "NL"),
        orderedAt,
        [new OrderItem(Guid.NewGuid(), "Product", 10m, 2)]);
}
