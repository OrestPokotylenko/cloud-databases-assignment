using System.Net.Mail;

namespace OnlineStore.Domain.Customers;

public record CustomerDetails
{
    public const int MaximumNameLength = 100;
    public const int MaximumEmailLength = 320;

    public string FirstName { get; }
    public string LastName { get; }
    public string Email { get; }

    public CustomerDetails(
        string firstName,
        string lastName,
        string email)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(firstName);
        ArgumentException.ThrowIfNullOrWhiteSpace(lastName);
        ArgumentException.ThrowIfNullOrWhiteSpace(email);

        FirstName = firstName.Trim();
        LastName = lastName.Trim();
        Email = email.Trim().ToLowerInvariant();

        if (FirstName.Length > MaximumNameLength)
        {
            throw new ArgumentException(
                $"The first name cannot exceed {MaximumNameLength} characters.",
                nameof(firstName));
        }

        if (LastName.Length > MaximumNameLength)
        {
            throw new ArgumentException(
                $"The last name cannot exceed {MaximumNameLength} characters.",
                nameof(lastName));
        }

        if (Email.Length > MaximumEmailLength ||
            !MailAddress.TryCreate(Email, out var parsedEmail) ||
            !string.Equals(parsedEmail.Address, Email, StringComparison.OrdinalIgnoreCase))
        {
            throw new ArgumentException("The email address is invalid.", nameof(email));
        }
    }
}
