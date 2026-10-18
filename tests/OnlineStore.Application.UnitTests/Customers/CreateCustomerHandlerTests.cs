using OnlineStore.Application.Customers.Abstractions;
using OnlineStore.Application.Customers.CreateCustomer;

using Xunit;

namespace OnlineStore.Application.UnitTests.Customers;

public class CreateCustomerHandlerTests
{
    [Fact]
    public async Task HandleAsync_ValidCustomer_CreatesCustomer()
    {
        var commandRepository = new CustomerCommandRepositoryStub();
        var queryRepository = new CustomerQueryRepositoryStub();
        var handler = new CreateCustomerHandler(
            commandRepository,
            queryRepository);

        var result = await handler.HandleAsync(
            new CreateCustomerCommand(
                " John ",
                " Smith ",
                " JOHN@EXAMPLE.COM "),
            TestContext.Current.CancellationToken);

        Assert.Equal(commandRepository.AddedPublicId, result.PublicId);
        Assert.NotNull(commandRepository.AddedCustomer);
        Assert.Equal("John", commandRepository.AddedCustomer.FirstName);
        Assert.Equal("Smith", commandRepository.AddedCustomer.LastName);
        Assert.Equal("john@example.com", commandRepository.AddedCustomer.Email);
        Assert.Equal("john@example.com", queryRepository.RequestedEmail);
    }

    [Fact]
    public async Task HandleAsync_InvalidEmail_RejectsCustomer()
    {
        var handler = new CreateCustomerHandler(
            new CustomerCommandRepositoryStub(),
            new CustomerQueryRepositoryStub());

        await Assert.ThrowsAsync<ArgumentException>(() =>
            handler.HandleAsync(
                new CreateCustomerCommand("John", "Smith", "invalid"),
                TestContext.Current.CancellationToken));
    }

    [Fact]
    public async Task HandleAsync_DuplicateEmail_DoesNotCreateCustomer()
    {
        var existing = new CustomerData(
            Guid.NewGuid(),
            "Existing",
            "Customer",
            "john@example.com",
            DateTimeOffset.UtcNow,
            null);
        var commandRepository = new CustomerCommandRepositoryStub();
        var queryRepository = new CustomerQueryRepositoryStub
        {
            CustomerByEmail = existing
        };
        var handler = new CreateCustomerHandler(
            commandRepository,
            queryRepository);

        var result = await handler.HandleAsync(
            new CreateCustomerCommand("John", "Smith", "john@example.com"),
            TestContext.Current.CancellationToken);

        Assert.Null(result.PublicId);
        Assert.Null(commandRepository.AddedCustomer);
    }
}
