using OnlineStore.Application.Customers.Abstractions;
using OnlineStore.Application.Customers.GetCustomer;

using Xunit;

namespace OnlineStore.Application.UnitTests.Customers;

public class GetCustomerHandlerTests
{
    [Fact]
    public async Task HandleAsync_ExistingCustomer_ReturnsCustomer()
    {
        var publicId = Guid.NewGuid();
        var repository = new CustomerQueryRepositoryStub
        {
            CustomerByPublicId = new CustomerData(
                publicId,
                "John",
                "Smith",
                "john@example.com",
                DateTimeOffset.UtcNow,
                null)
        };
        var handler = new GetCustomerHandler(repository);

        var result = await handler.HandleAsync(
            new GetCustomerQuery(publicId),
            TestContext.Current.CancellationToken);

        Assert.NotNull(result);
        Assert.Equal(publicId, result.PublicId);
        Assert.Equal("John", result.FirstName);
        Assert.Equal("john@example.com", result.Email);
    }
}
