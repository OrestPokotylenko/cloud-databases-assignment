using Microsoft.EntityFrameworkCore;

using OnlineStore.Application.Customers.Abstractions;
using OnlineStore.Domain.Customers.Entities;

namespace OnlineStore.Persistence.Customers;

public class CustomerRepository(
    OnlineStoreDbContext context)
    : ICustomerCommandRepository, ICustomerQueryRepository
{
    public async Task<Guid> AddAsync(
        Customer customer,
        CancellationToken ct)
    {
        context.Customers.Add(customer);
        await context.SaveChangesAsync(ct);

        return customer.PublicId;
    }

    public Task<CustomerData?> GetByPublicIdAsync(
        Guid publicId,
        CancellationToken ct) =>
        context.Customers
            .AsNoTracking()
            .Where(customer => customer.PublicId == publicId)
            .Select(Map())
            .SingleOrDefaultAsync(ct);

    public Task<CustomerData?> GetByEmailAsync(
        string email,
        CancellationToken ct)
    {
        var normalizedEmail = email.Trim().ToLowerInvariant();

        return context.Customers
            .AsNoTracking()
            .Where(customer => customer.Email == normalizedEmail)
            .Select(Map())
            .SingleOrDefaultAsync(ct);
    }

    public async Task<bool> UpdateAsync(
        CustomerUpdate customerUpdate,
        CancellationToken ct)
    {
        var customer = await context.Customers.SingleOrDefaultAsync(
            customer => customer.PublicId == customerUpdate.PublicId,
            ct);
        if (customer is null)
        {
            return false;
        }

        customer.Update(
            customerUpdate.Details,
            customerUpdate.UpdatedAt);
        await context.SaveChangesAsync(ct);

        return true;
    }

    private static System.Linq.Expressions.Expression<Func<Customer, CustomerData>> Map() =>
        customer => new CustomerData(
            customer.PublicId,
            customer.FirstName,
            customer.LastName,
            customer.Email,
            customer.CreatedAt,
            customer.UpdatedAt);
}
