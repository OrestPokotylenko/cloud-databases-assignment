using OnlineStore.Application.Customers.Abstractions;
using OnlineStore.Domain.Customers.Entities;

namespace OnlineStore.Application.UnitTests.Customers;

internal class CustomerCommandRepositoryStub : ICustomerCommandRepository
{
    public Guid AddedPublicId { get; set; } = Guid.NewGuid();
    public bool UpdateResult { get; set; } = true;
    public Customer? AddedCustomer { get; private set; }
    public CustomerUpdate? ReceivedUpdate { get; private set; }
    public CancellationToken ReceivedCancellationToken { get; private set; }

    public Task<Guid> AddAsync(
        Customer customer,
        CancellationToken ct)
    {
        AddedCustomer = customer;
        ReceivedCancellationToken = ct;
        return Task.FromResult(AddedPublicId);
    }

    public Task<bool> UpdateAsync(
        CustomerUpdate customerUpdate,
        CancellationToken ct)
    {
        ReceivedUpdate = customerUpdate;
        ReceivedCancellationToken = ct;
        return Task.FromResult(UpdateResult);
    }
}
