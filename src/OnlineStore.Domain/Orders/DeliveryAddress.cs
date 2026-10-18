namespace OnlineStore.Domain.Orders;

public record DeliveryAddress
{
    public DeliveryAddress(
        string street,
        string houseNumber,
        string? addition,
        string postalCode,
        string city,
        string countryCode)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(street);
        ArgumentException.ThrowIfNullOrWhiteSpace(houseNumber);
        ArgumentException.ThrowIfNullOrWhiteSpace(postalCode);
        ArgumentException.ThrowIfNullOrWhiteSpace(city);
        ArgumentException.ThrowIfNullOrWhiteSpace(countryCode);

        Street = street.Trim();
        HouseNumber = houseNumber.Trim();
        Addition = string.IsNullOrWhiteSpace(addition) ? null : addition.Trim();
        PostalCode = postalCode.Trim().ToUpperInvariant();
        City = city.Trim();
        CountryCode = countryCode.Trim().ToUpperInvariant();

        if (Street.Length > 200 || HouseNumber.Length > 20 || Addition?.Length > 20 ||
            PostalCode.Length > 20 || City.Length > 100 || CountryCode.Length != 2)
        {
            throw new ArgumentException("The delivery address is invalid.");
        }
    }

    public string Street { get; }
    public string HouseNumber { get; }
    public string? Addition { get; }
    public string PostalCode { get; }
    public string City { get; }
    public string CountryCode { get; }
}
