using OnlineStore.Domain.Orders;

namespace OnlineStore.Application.Orders.Abstractions;

public record OrderData(
    Guid PublicId,
    Guid CustomerPublicId,
    string CustomerFullName,
    string CustomerEmail,
    string? CustomerPhoneNumber,
    string Street,
    string HouseNumber,
    string? AddressAddition,
    string PostalCode,
    string City,
    string CountryCode,
    OrderStatus Status,
    DateTimeOffset OrderDate,
    DateTimeOffset UpdatedAt,
    DateTimeOffset? ShippingDate,
    TimeSpan? ProcessingDuration,
    decimal TotalAmount,
    string? RejectionReason,
    IReadOnlyList<OrderItemData> Items);
