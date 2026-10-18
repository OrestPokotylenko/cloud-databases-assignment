using System.ComponentModel.DataAnnotations;

namespace OnlineStore.API.Orders;

public class DeliveryAddressRequest
{
    [Required]
    [StringLength(200)]
    public required string Street { get; init; }

    [Required]
    [StringLength(20)]
    public required string HouseNumber { get; init; }

    [StringLength(20)]
    public string? Addition { get; init; }

    [Required]
    [StringLength(20)]
    public required string PostalCode { get; init; }

    [Required]
    [StringLength(100)]
    public required string City { get; init; }

    [Required]
    [StringLength(2, MinimumLength = 2)]
    public required string CountryCode { get; init; }
}
