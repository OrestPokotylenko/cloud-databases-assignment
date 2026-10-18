namespace OnlineStore.Application.Customers;

public record CustomerResult(
    Guid PublicId,
    string FirstName,
    string LastName,
    string Email,
    DateTimeOffset CreatedAt,
    DateTimeOffset? UpdatedAt);
