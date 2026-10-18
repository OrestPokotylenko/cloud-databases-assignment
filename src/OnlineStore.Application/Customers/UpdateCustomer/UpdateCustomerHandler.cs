using OnlineStore.Application.Abstractions.CQRS;
using OnlineStore.Application.Customers.Abstractions;
using OnlineStore.Domain.Customers;

namespace OnlineStore.Application.Customers.UpdateCustomer;

public class UpdateCustomerHandler(
    ICustomerCommandRepository commandRepository,
    ICustomerQueryRepository queryRepository,
    TimeProvider timeProvider)
    : IHandler<UpdateCustomerCommand, UpdateCustomerResult>
{
    public async Task<UpdateCustomerResult> HandleAsync(
        UpdateCustomerCommand request,
        CancellationToken ct)
    {
        ArgumentOutOfRangeException.ThrowIfEqual(request.PublicId, Guid.Empty);

        if (request.FirstName is null &&
            request.LastName is null &&
            request.Email is null)
        {
            throw new ArgumentException("At least one customer field must be provided.");
        }

        var existing = await queryRepository.GetByPublicIdAsync(
            request.PublicId,
            ct);
        if (existing is null)
        {
            return new UpdateCustomerResult(false, true);
        }

        var details = new CustomerDetails(
            request.FirstName ?? existing.FirstName,
            request.LastName ?? existing.LastName,
            request.Email ?? existing.Email);

        if (!string.Equals(details.Email, existing.Email, StringComparison.Ordinal) &&
            await queryRepository.GetByEmailAsync(details.Email, ct) is not null)
        {
            return new UpdateCustomerResult(true, false);
        }

        var updated = await commandRepository.UpdateAsync(
            new CustomerUpdate(
                request.PublicId,
                details,
                timeProvider.GetUtcNow()),
            ct);

        return new UpdateCustomerResult(updated, true);
    }
}
