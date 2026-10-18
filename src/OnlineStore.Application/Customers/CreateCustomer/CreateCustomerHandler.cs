using OnlineStore.Application.Abstractions.CQRS;
using OnlineStore.Application.Customers.Abstractions;
using OnlineStore.Domain.Customers;
using OnlineStore.Domain.Customers.Entities;

namespace OnlineStore.Application.Customers.CreateCustomer;

public class CreateCustomerHandler(
    ICustomerCommandRepository commandRepository,
    ICustomerQueryRepository queryRepository)
    : IHandler<CreateCustomerCommand, CreateCustomerResult>
{
    public async Task<CreateCustomerResult> HandleAsync(
        CreateCustomerCommand request,
        CancellationToken ct)
    {
        var details = new CustomerDetails(
            request.FirstName,
            request.LastName,
            request.Email);

        if (await queryRepository.GetByEmailAsync(details.Email, ct) is not null)
        {
            return new CreateCustomerResult(null);
        }

        var publicId = await commandRepository.AddAsync(
            new Customer(details),
            ct);

        return new CreateCustomerResult(publicId);
    }
}
