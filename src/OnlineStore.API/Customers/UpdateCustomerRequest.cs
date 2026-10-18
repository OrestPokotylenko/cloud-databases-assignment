using System.ComponentModel.DataAnnotations;

using OnlineStore.Domain.Customers;

namespace OnlineStore.API.Customers;

public class UpdateCustomerRequest
{
    [StringLength(CustomerDetails.MaximumNameLength, MinimumLength = 1)]
    public string? FirstName { get; init; }

    [StringLength(CustomerDetails.MaximumNameLength, MinimumLength = 1)]
    public string? LastName { get; init; }

    [EmailAddress]
    [StringLength(CustomerDetails.MaximumEmailLength, MinimumLength = 1)]
    public string? Email { get; init; }
}
