using OnlineStore.Domain.Customers.Entities;

namespace OnlineStore.Application.Customers.Abstractions;

public interface ICustomerCommandRepository
{
    Task<Guid> AddAsync(
        Customer customer,
        CancellationToken ct);

    Task<bool> UpdateAsync(
        CustomerUpdate customerUpdate,
        CancellationToken ct);
}
