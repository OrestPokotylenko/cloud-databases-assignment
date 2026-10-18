using OnlineStore.Application.Abstractions.CQRS;

namespace OnlineStore.Application.Customers.GetCustomer;

public record GetCustomerQuery(Guid PublicId)
    : IQuery<CustomerResult?>;
