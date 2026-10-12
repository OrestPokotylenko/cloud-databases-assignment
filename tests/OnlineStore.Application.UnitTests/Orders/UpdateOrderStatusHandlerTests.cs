using OnlineStore.Application.Orders.Messages;
using OnlineStore.Application.Orders.UpdateOrderStatus;
using OnlineStore.Domain.Orders;

using Xunit;

namespace OnlineStore.Application.UnitTests.Orders;

public class UpdateOrderStatusHandlerTests
{
    [Theory]
    [InlineData("processing")]
    [InlineData("Processing")]
    [InlineData("PROCESSING")]
    public async Task HandleAsync_StatusCasingDoesNotMatter(string status)
    {
        var repository = new OrderQueryRepositoryStub { Status = OrderStatus.Pending };
        var publisher = new MessagePublisherStub<UpdateOrderStatusMessage>();
        var handler = new UpdateOrderStatusHandler(repository, publisher, TimeProvider.System);

        var result = await handler.HandleAsync(
            new UpdateOrderStatusCommand(Guid.NewGuid(), status),
            TestContext.Current.CancellationToken);

        Assert.True(result.Accepted);
        Assert.Equal(OrderStatus.Processing, publisher.PublishedMessage!.Status);
    }

    [Fact]
    public async Task HandleAsync_MissingOrder_DoesNotPublish()
    {
        var repository = new OrderQueryRepositoryStub();
        var publisher = new MessagePublisherStub<UpdateOrderStatusMessage>();
        var handler = new UpdateOrderStatusHandler(repository, publisher, TimeProvider.System);

        var result = await handler.HandleAsync(
            new UpdateOrderStatusCommand(Guid.NewGuid(), "cancelled"),
            TestContext.Current.CancellationToken);

        Assert.False(result.Accepted);
        Assert.Null(publisher.PublishedMessage);
    }

    [Fact]
    public async Task HandleAsync_RejectedStatus_RejectsRequest()
    {
        var repository = new OrderQueryRepositoryStub { Status = OrderStatus.Pending };
        var handler = new UpdateOrderStatusHandler(
            repository,
            new MessagePublisherStub<UpdateOrderStatusMessage>(),
            TimeProvider.System);

        await Assert.ThrowsAsync<ArgumentException>(() => handler.HandleAsync(
            new UpdateOrderStatusCommand(Guid.NewGuid(), "rejected"),
            TestContext.Current.CancellationToken));
    }

    [Fact]
    public async Task HandleAsync_NumericStatus_RejectsRequest()
    {
        var repository = new OrderQueryRepositoryStub { Status = OrderStatus.Pending };
        var handler = new UpdateOrderStatusHandler(
            repository,
            new MessagePublisherStub<UpdateOrderStatusMessage>(),
            TimeProvider.System);

        await Assert.ThrowsAsync<ArgumentException>(() => handler.HandleAsync(
            new UpdateOrderStatusCommand(Guid.NewGuid(), "1"),
            TestContext.Current.CancellationToken));
    }
}
