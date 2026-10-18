namespace OnlineStore.Domain.Customers.Entities;

public class Customer
{
    private string? firstName;
    private string? lastName;
    private string? email;

    public int Id { get; private set; }
    public Guid PublicId { get; private set; }
    public string FirstName
    {
        get => firstName ?? throw new InvalidOperationException("The first name has not been initialized.");
        private set => firstName = value;
    }
    public string LastName
    {
        get => lastName ?? throw new InvalidOperationException("The last name has not been initialized.");
        private set => lastName = value;
    }
    public string Email
    {
        get => email ?? throw new InvalidOperationException("The email has not been initialized.");
        private set => email = value;
    }
    public DateTimeOffset CreatedAt { get; private set; }
    public DateTimeOffset? UpdatedAt { get; private set; }

    private Customer() { }

    public Customer(CustomerDetails details)
    {
        SetDetails(details);
    }

    public void Update(
        CustomerDetails details,
        DateTimeOffset updatedAt)
    {
        if (updatedAt == default)
        {
            throw new ArgumentOutOfRangeException(nameof(updatedAt));
        }

        SetDetails(details);
        UpdatedAt = updatedAt;
    }

    private void SetDetails(CustomerDetails details)
    {
        ArgumentNullException.ThrowIfNull(details);

        FirstName = details.FirstName;
        LastName = details.LastName;
        Email = details.Email;
    }
}
