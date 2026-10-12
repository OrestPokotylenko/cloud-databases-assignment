namespace OnlineStore.Application.Orders.Messages;

public record DeliveryAddressMessage(
    string Street,
    string HouseNumber,
    string? Addition,
    string PostalCode,
    string City,
    string CountryCode);
