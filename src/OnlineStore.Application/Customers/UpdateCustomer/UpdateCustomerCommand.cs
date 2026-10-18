using OnlineStore.Application.Abstractions.CQRS;

namespace OnlineStore.Application.Customers.UpdateCustomer;

public record UpdateCustomerCommand(
    Guid PublicId,
    string? FirstName,
    string? LastName,
    string? Email) : ICommand<UpdateCustomerResult>;
