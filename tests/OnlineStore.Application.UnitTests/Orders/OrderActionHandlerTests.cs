using OnlineStore.Application.Orders.Actions;
using OnlineStore.Application.Orders.CancelOrder;
using OnlineStore.Application.Orders.Messages;
using OnlineStore.Application.Orders.ProcessOrder;
using OnlineStore.Application.Orders.ShipOrder;

using Xunit;

namespace OnlineStore.Application.UnitTests.Orders;

public class OrderActionHandlerTests
{
    [Fact]
    public async Task HandleAsync_ProcessOrder_PublishesProcessAction()
    {
        var (handler, publisher) = CreateHandler(true);

        var result = await handler.HandleAsync(
            new ProcessOrderCommand(Guid.NewGuid()),
            TestContext.Current.CancellationToken);

        Assert.True(result.Accepted);
        Assert.Equal(OrderAction.Process, publisher.PublishedMessage!.Action);
    }

    [Fact]
    public async Task HandleAsync_ShipOrder_PublishesShipAction()
    {
        var (handler, publisher) = CreateHandler(true);

        var result = await handler.HandleAsync(
            new ShipOrderCommand(Guid.NewGuid()),
            TestContext.Current.CancellationToken);

        Assert.True(result.Accepted);
        Assert.Equal(OrderAction.Ship, publisher.PublishedMessage!.Action);
    }

    [Fact]
    public async Task HandleAsync_CancelOrder_PublishesCancelAction()
    {
        var (handler, publisher) = CreateHandler(true);

        var result = await handler.HandleAsync(
            new CancelOrderCommand(Guid.NewGuid()),
            TestContext.Current.CancellationToken);

        Assert.True(result.Accepted);
        Assert.Equal(OrderAction.Cancel, publisher.PublishedMessage!.Action);
    }

    [Fact]
    public async Task HandleAsync_MissingOrder_DoesNotPublish()
    {
        var (handler, publisher) = CreateHandler(false);

        var result = await handler.HandleAsync(
            new ProcessOrderCommand(Guid.NewGuid()),
            TestContext.Current.CancellationToken);

        Assert.False(result.Accepted);
        Assert.Null(publisher.PublishedMessage);
    }

    private static (
        OrderActionHandler Handler,
        MessagePublisherStub<OrderActionMessage> Publisher) CreateHandler(
        bool orderExists)
    {
        var publisher = new MessagePublisherStub<OrderActionMessage>();
        var handler = new OrderActionHandler(
            new OrderQueryRepositoryStub { Exists = orderExists },
            publisher,
            TimeProvider.System);

        return (handler, publisher);
    }
}
