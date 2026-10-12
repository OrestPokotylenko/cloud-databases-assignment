using System.ComponentModel.DataAnnotations;

namespace OnlineStore.API.Orders;

public class OrderCustomerRequest
{
    [Required]
    [StringLength(200)]
    public required string FullName { get; init; }

    [Required]
    [EmailAddress]
    [StringLength(320)]
    public required string Email { get; init; }

    [StringLength(30)]
    public string? PhoneNumber { get; init; }
}
