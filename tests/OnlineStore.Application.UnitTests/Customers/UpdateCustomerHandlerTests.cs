using OnlineStore.Application.Customers.Abstractions;
using OnlineStore.Application.Customers.UpdateCustomer;

using Xunit;

namespace OnlineStore.Application.UnitTests.Customers;

public class UpdateCustomerHandlerTests
{
    [Fact]
    public async Task HandleAsync_ValidChanges_UpdatesCustomer()
    {
        var publicId = Guid.NewGuid();
        var updatedAt = DateTimeOffset.UtcNow;
        var queryRepository = new CustomerQueryRepositoryStub
        {
            CustomerByPublicId = CreateCustomer(publicId)
        };
        var commandRepository = new CustomerCommandRepositoryStub();
        var handler = new UpdateCustomerHandler(
            commandRepository,
            queryRepository,
            new FixedTimeProvider(updatedAt));

        var result = await handler.HandleAsync(
            new UpdateCustomerCommand(
                publicId,
                "Jane",
                null,
                "jane@example.com"),
            TestContext.Current.CancellationToken);

        Assert.True(result.CustomerExists);
        Assert.True(result.EmailAvailable);
        Assert.NotNull(commandRepository.ReceivedUpdate);
        Assert.Equal("Jane", commandRepository.ReceivedUpdate.Details.FirstName);
        Assert.Equal("Smith", commandRepository.ReceivedUpdate.Details.LastName);
        Assert.Equal("jane@example.com", commandRepository.ReceivedUpdate.Details.Email);
        Assert.Equal(updatedAt, commandRepository.ReceivedUpdate.UpdatedAt);
    }

    [Fact]
    public async Task HandleAsync_DuplicateEmail_DoesNotUpdateCustomer()
    {
        var publicId = Guid.NewGuid();
        var queryRepository = new CustomerQueryRepositoryStub
        {
            CustomerByPublicId = CreateCustomer(publicId),
            CustomerByEmail = CreateCustomer(Guid.NewGuid()) with
            {
                Email = "duplicate@example.com"
            }
        };
        var commandRepository = new CustomerCommandRepositoryStub();
        var handler = new UpdateCustomerHandler(
            commandRepository,
            queryRepository,
            new FixedTimeProvider(DateTimeOffset.UtcNow));

        var result = await handler.HandleAsync(
            new UpdateCustomerCommand(
                publicId,
                null,
                null,
                "duplicate@example.com"),
            TestContext.Current.CancellationToken);

        Assert.True(result.CustomerExists);
        Assert.False(result.EmailAvailable);
        Assert.Null(commandRepository.ReceivedUpdate);
    }

    private static CustomerData CreateCustomer(Guid publicId) => new(
        publicId,
        "John",
        "Smith",
        "john@example.com",
        DateTimeOffset.UtcNow,
        null);
}
