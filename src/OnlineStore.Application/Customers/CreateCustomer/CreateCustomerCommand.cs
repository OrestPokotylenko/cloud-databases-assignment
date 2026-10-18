using OnlineStore.Application.Abstractions.CQRS;

namespace OnlineStore.Application.Customers.CreateCustomer;

public record CreateCustomerCommand(
    string FirstName,
    string LastName,
    string Email) : ICommand<CreateCustomerResult>;
