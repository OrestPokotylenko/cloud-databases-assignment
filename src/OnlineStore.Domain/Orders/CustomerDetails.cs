namespace OnlineStore.Domain.Orders;

public record CustomerDetails
{
    public CustomerDetails(
        string fullName,
        string email,
        string? phoneNumber)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(fullName);
        ArgumentException.ThrowIfNullOrWhiteSpace(email);

        FullName = fullName.Trim();
        Email = email.Trim().ToLowerInvariant();
        PhoneNumber = string.IsNullOrWhiteSpace(phoneNumber)
            ? null
            : phoneNumber.Trim();

        if (FullName.Length > 200 || Email.Length > 320 || PhoneNumber?.Length > 30)
        {
            throw new ArgumentException("The customer details exceed the allowed length.");
        }
    }

    public string FullName { get; }
    public string Email { get; }
    public string? PhoneNumber { get; }
}
