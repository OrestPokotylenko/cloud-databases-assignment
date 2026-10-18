using OnlineStore.Application.Abstractions.CQRS;
using OnlineStore.Application.Customers.Abstractions;

namespace OnlineStore.Application.Customers.GetCustomer;

public class GetCustomerHandler(
    ICustomerQueryRepository customerRepository)
    : IHandler<GetCustomerQuery, CustomerResult?>
{
    public async Task<CustomerResult?> HandleAsync(
        GetCustomerQuery request,
        CancellationToken ct)
    {
        ArgumentOutOfRangeException.ThrowIfEqual(request.PublicId, Guid.Empty);

        var customer = await customerRepository.GetByPublicIdAsync(
            request.PublicId,
            ct);

        return customer is null ? null : CustomerResultMapper.Map(customer);
    }
}
