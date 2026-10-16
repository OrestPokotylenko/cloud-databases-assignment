using OnlineStore.Domain.Orders;

namespace OnlineStore.Application.Orders.GetOrderById;

public record GetOrderByIdResult(
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
    double? ProcessingDurationSeconds,
    decimal TotalAmount,
    string? RejectionReason,
    IReadOnlyList<OrderItemResult> Items);
