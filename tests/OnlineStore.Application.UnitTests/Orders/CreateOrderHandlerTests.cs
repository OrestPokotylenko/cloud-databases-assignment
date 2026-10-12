using OnlineStore.Application.Orders.CreateOrder;
using OnlineStore.Application.Orders.Messages;

using Xunit;

namespace OnlineStore.Application.UnitTests.Orders;

public class CreateOrderHandlerTests
{
    [Fact]
    public async Task HandleAsync_ValidOrder_PublishesCreateMessage()
    {
        var now = new DateTimeOffset(2026, 10, 11, 10, 0, 0, TimeSpan.Zero);
        var publisher = new MessagePublisherStub<CreateOrderMessage>();
        var handler = new CreateOrderHandler(publisher, new FixedTimeProvider(now));
        var productId = Guid.NewGuid();

        var result = await handler.HandleAsync(
            new CreateOrderCommand(
                Guid.NewGuid(),
                new OrderCustomerMessage("Customer", "customer@example.com", null),
                new DeliveryAddressMessage("Street", "1", null, "1000 AA", "Amsterdam", "nl"),
                [new OrderItemMessage(productId, 2)]),
            TestContext.Current.CancellationToken);

        Assert.NotEqual(Guid.Empty, result.PublicId);
        Assert.Equal(4, result.PublicId.Version);
        Assert.NotNull(publisher.PublishedMessage);
        Assert.Equal(result.PublicId, publisher.PublishedMessage.OrderPublicId);
        Assert.Equal(now, publisher.PublishedMessage.SubmittedAt);
        Assert.Equal(productId, Assert.Single(publisher.PublishedMessage.Items).ProductPublicId);
    }

    [Fact]
    public async Task HandleAsync_DuplicateProducts_CombinesQuantities()
    {
        var publisher = new MessagePublisherStub<CreateOrderMessage>();
        var handler = new CreateOrderHandler(publisher, TimeProvider.System);
        var productId = Guid.NewGuid();

        await handler.HandleAsync(
            new CreateOrderCommand(
                Guid.NewGuid(),
                new OrderCustomerMessage("Customer", "customer@example.com", null),
                new DeliveryAddressMessage("Street", "1", null, "1000 AA", "Amsterdam", "NL"),
                [new OrderItemMessage(productId, 2), new OrderItemMessage(productId, 3)]),
            TestContext.Current.CancellationToken);

        Assert.Equal(5, Assert.Single(publisher.PublishedMessage!.Items).Quantity);
    }

    [Fact]
    public async Task HandleAsync_MoreThanOneHundredItems_PublishesCreateMessage()
    {
        var publisher = new MessagePublisherStub<CreateOrderMessage>();
        var handler = new CreateOrderHandler(publisher, TimeProvider.System);
        var items = Enumerable.Range(0, 101)
            .Select(_ => new OrderItemMessage(Guid.NewGuid(), 1))
            .ToList();

        await handler.HandleAsync(
            new CreateOrderCommand(
                Guid.NewGuid(),
                new OrderCustomerMessage("Customer", "customer@example.com", null),
                new DeliveryAddressMessage("Street", "1", null, "1000 AA", "Amsterdam", "NL"),
                items),
            TestContext.Current.CancellationToken);

        Assert.Equal(101, publisher.PublishedMessage!.Items.Count);
    }
}
