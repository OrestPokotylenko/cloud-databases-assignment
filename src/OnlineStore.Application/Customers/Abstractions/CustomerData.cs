namespace OnlineStore.Application.Customers.Abstractions;

public record CustomerData(
    Guid PublicId,
    string FirstName,
    string LastName,
    string Email,
    DateTimeOffset CreatedAt,
    DateTimeOffset? UpdatedAt);
