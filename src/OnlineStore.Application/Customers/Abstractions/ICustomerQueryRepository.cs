namespace OnlineStore.Application.Customers.Abstractions;

public interface ICustomerQueryRepository
{
    Task<CustomerData?> GetByPublicIdAsync(
        Guid publicId,
        CancellationToken ct);

    Task<CustomerData?> GetByEmailAsync(
        string email,
        CancellationToken ct);
}
