using OnlineStore.Application.Customers.Abstractions;

namespace OnlineStore.Application.Customers;

public static class CustomerResultMapper
{
    public static CustomerResult Map(CustomerData customer) =>
        new(
            customer.PublicId,
            customer.FirstName,
            customer.LastName,
            customer.Email,
            customer.CreatedAt,
            customer.UpdatedAt);
}
