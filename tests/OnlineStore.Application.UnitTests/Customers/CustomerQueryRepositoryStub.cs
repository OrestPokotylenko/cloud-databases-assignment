using OnlineStore.Application.Customers.Abstractions;

namespace OnlineStore.Application.UnitTests.Customers;

internal class CustomerQueryRepositoryStub : ICustomerQueryRepository
{
    public CustomerData? CustomerByPublicId { get; set; }
    public CustomerData? CustomerByEmail { get; set; }
    public Guid? RequestedPublicId { get; private set; }
    public string? RequestedEmail { get; private set; }
    public CancellationToken ReceivedCancellationToken { get; private set; }

    public Task<CustomerData?> GetByPublicIdAsync(
        Guid publicId,
        CancellationToken ct)
    {
        RequestedPublicId = publicId;
        ReceivedCancellationToken = ct;
        return Task.FromResult(CustomerByPublicId);
    }

    public Task<CustomerData?> GetByEmailAsync(
        string email,
        CancellationToken ct)
    {
        RequestedEmail = email;
        ReceivedCancellationToken = ct;
        return Task.FromResult(CustomerByEmail);
    }
}
