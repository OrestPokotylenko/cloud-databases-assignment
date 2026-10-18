using OnlineStore.Application.Abstractions.CQRS;
using OnlineStore.Application.Abstractions.Messaging;
using OnlineStore.Application.Customers.Abstractions;
using OnlineStore.Application.Orders.Messages;
using OnlineStore.Domain.Orders;

namespace OnlineStore.Application.Orders.CreateOrder;

public class CreateOrderHandler(
    IMessagePublisher<CreateOrderMessage> publisher,
    ICustomerQueryRepository customerRepository,
    TimeProvider timeProvider)
    : IHandler<CreateOrderCommand, CreateOrderResult?>
{
    public async Task<CreateOrderResult?> HandleAsync(
        CreateOrderCommand request,
        CancellationToken ct)
    {
        ArgumentNullException.ThrowIfNull(request.DeliveryAddress);
        ArgumentNullException.ThrowIfNull(request.Items);

        if (request.CustomerPublicId == Guid.Empty)
        {
            throw new ArgumentOutOfRangeException(nameof(request.CustomerPublicId));
        }

        _ = new DeliveryAddress(
            request.DeliveryAddress.Street,
            request.DeliveryAddress.HouseNumber,
            request.DeliveryAddress.Addition,
            request.DeliveryAddress.PostalCode,
            request.DeliveryAddress.City,
            request.DeliveryAddress.CountryCode);

        if (request.Items.Count == 0)
        {
            throw new ArgumentException("An order must contain at least one item.");
        }

        if (request.Items.Any(item =>
                item.ProductPublicId == Guid.Empty || item.Quantity < 1))
        {
            throw new ArgumentException("Every order item must contain a product and a positive quantity.");
        }

        if (await customerRepository.GetByPublicIdAsync(
                request.CustomerPublicId,
                ct) is null)
        {
            return null;
        }

        var items = request.Items
            .GroupBy(item => item.ProductPublicId)
            .Select(group => new OrderItemMessage(
                group.Key,
                checked(group.Sum(item => item.Quantity))))
            .ToList();

        var publicId = Guid.NewGuid();
        var message = new CreateOrderMessage(
            Guid.NewGuid(),
            Guid.NewGuid(),
            timeProvider.GetUtcNow(),
            publicId,
            request.CustomerPublicId,
            request.DeliveryAddress,
            items);

        await publisher.PublishAsync(message, ct);
        return new CreateOrderResult(publicId);
    }
}
