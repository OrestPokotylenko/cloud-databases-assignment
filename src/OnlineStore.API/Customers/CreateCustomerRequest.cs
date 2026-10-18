using System.ComponentModel.DataAnnotations;

using OnlineStore.Domain.Customers;

namespace OnlineStore.API.Customers;

public class CreateCustomerRequest
{
    [Required]
    [StringLength(CustomerDetails.MaximumNameLength)]
    public required string FirstName { get; init; }

    [Required]
    [StringLength(CustomerDetails.MaximumNameLength)]
    public required string LastName { get; init; }

    [Required]
    [EmailAddress]
    [StringLength(CustomerDetails.MaximumEmailLength)]
    public required string Email { get; init; }
}
