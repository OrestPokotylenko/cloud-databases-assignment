using OnlineStore.Domain.Customers;

namespace OnlineStore.Application.Customers.Abstractions;

public record CustomerUpdate(
    Guid PublicId,
    CustomerDetails Details,
    DateTimeOffset UpdatedAt);
