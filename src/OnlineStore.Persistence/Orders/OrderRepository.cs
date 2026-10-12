using System.Data;

using Microsoft.EntityFrameworkCore;

using OnlineStore.Application.Orders.Abstractions;
using OnlineStore.Application.Orders.Messages;
using OnlineStore.Domain.Orders;
using OnlineStore.Domain.Orders.Entities;
using OnlineStore.Persistence.Messaging;

namespace OnlineStore.Persistence.Orders;

public class OrderRepository(
    OnlineStoreDbContext context,
    TimeProvider timeProvider)
    : IOrderCommandRepository, IOrderQueryRepository
{
    public async Task CreateAsync(
        CreateOrderMessage message,
        CancellationToken ct)
    {
        await using var transaction = await context.Database.BeginTransactionAsync(
            IsolationLevel.Serializable,
            ct);

        if (await IsProcessedAsync(message.MessageId, ct))
        {
            return;
        }

        if (await context.Orders.AnyAsync(
                order => order.PublicId == message.OrderPublicId,
                ct))
        {
            AddProcessedMessage(message.MessageId, nameof(CreateOrderMessage));
            await context.SaveChangesAsync(ct);
            await transaction.CommitAsync(ct);
            return;
        }

        var productIds = message.Items
            .Select(item => item.ProductPublicId)
            .ToList();
        var products = await context.Products
            .Where(product => productIds.Contains(product.PublicId))
            .OrderBy(product => product.Id)
            .ToListAsync(ct);
        var requestedQuantities = message.Items.ToDictionary(
            item => item.ProductPublicId,
            item => item.Quantity);

        var canFulfil = products.Count == productIds.Count &&
            products.All(product =>
                product.StockQuantity >= requestedQuantities[product.PublicId]);

        var customer = new CustomerDetails(
            message.Customer.FullName,
            message.Customer.Email,
            message.Customer.PhoneNumber);
        var address = new DeliveryAddress(
            message.DeliveryAddress.Street,
            message.DeliveryAddress.HouseNumber,
            message.DeliveryAddress.Addition,
            message.DeliveryAddress.PostalCode,
            message.DeliveryAddress.City,
            message.DeliveryAddress.CountryCode);

        Order order;
        if (canFulfil)
        {
            var orderItems = products.Select(product =>
            {
                var quantity = requestedQuantities[product.PublicId];
                product.DecreaseStock(quantity);
                return new OrderItem(
                    product.PublicId,
                    product.Name,
                    product.Price,
                    quantity);
            });

            order = new Order(
                message.OrderPublicId,
                message.CustomerPublicId,
                customer,
                address,
                message.SubmittedAt,
                orderItems);
        }
        else
        {
            order = Order.Rejected(
                message.OrderPublicId,
                message.CustomerPublicId,
                customer,
                address,
                message.SubmittedAt,
                "One or more products are unavailable or have insufficient stock.");
        }

        context.Orders.Add(order);
        AddProcessedMessage(message.MessageId, nameof(CreateOrderMessage));
        await context.SaveChangesAsync(ct);
        await transaction.CommitAsync(ct);
    }

    public async Task UpdateStatusAsync(
        UpdateOrderStatusMessage message,
        CancellationToken ct)
    {
        await using var transaction = await context.Database.BeginTransactionAsync(
            IsolationLevel.Serializable,
            ct);

        if (await IsProcessedAsync(message.MessageId, ct))
        {
            return;
        }

        var order = await context.Orders
            .Include(entity => entity.Items)
            .SingleOrDefaultAsync(
                entity => entity.PublicId == message.OrderPublicId,
                ct)
            ?? throw new InvalidOperationException("The order does not exist.");

        var changed = order.UpdateStatus(message.Status, message.RequestedAt);
        if (changed && message.Status == OrderStatus.Cancelled)
        {
            var productIds = order.Items
                .Select(item => item.ProductPublicId)
                .ToList();
            var products = await context.Products
                .Where(product => productIds.Contains(product.PublicId))
                .OrderBy(product => product.Id)
                .ToListAsync(ct);

            if (products.Count != productIds.Count)
            {
                throw new InvalidOperationException(
                    "The order cannot be cancelled because a product no longer exists.");
            }

            var quantities = order.Items.ToDictionary(
                item => item.ProductPublicId,
                item => item.Quantity);
            foreach (var product in products)
            {
                product.IncreaseStock(quantities[product.PublicId]);
            }
        }

        AddProcessedMessage(message.MessageId, nameof(UpdateOrderStatusMessage));
        await context.SaveChangesAsync(ct);
        await transaction.CommitAsync(ct);
    }

    public Task<OrderStatus?> GetStatusAsync(
        Guid publicId,
        CancellationToken ct)
    {
        return context.Orders
            .AsNoTracking()
            .Where(order => order.PublicId == publicId)
            .Select(order => (OrderStatus?)order.Status)
            .SingleOrDefaultAsync(ct);
    }

    public async Task<OrderData?> GetByPublicIdAsync(
        Guid publicId,
        CancellationToken ct)
    {
        var order = await BaseQuery()
            .SingleOrDefaultAsync(order => order.PublicId == publicId, ct);

        return order is null ? null : Map(order);
    }

    public async Task<OrderPageData> GetPageAsync(
        int skip,
        int take,
        CancellationToken ct)
    {
        var totalCount = await context.Orders.CountAsync(ct);
        var orders = await BaseQuery()
            .OrderByDescending(order => order.OrderedAt)
            .ThenByDescending(order => order.Id)
            .Skip(skip)
            .Take(take)
            .ToListAsync(ct);

        return new OrderPageData(orders.Select(Map).ToList(), totalCount);
    }

    private IQueryable<Order> BaseQuery() => context.Orders
        .AsNoTracking()
        .Include(order => order.Items)
        .AsSplitQuery();

    private Task<bool> IsProcessedAsync(
        Guid messageId,
        CancellationToken ct) =>
        context.ProcessedMessages.AnyAsync(
            message => message.MessageId == messageId,
            ct);

    private void AddProcessedMessage(
        Guid messageId,
        string messageType)
    {
        context.ProcessedMessages.Add(
            new ProcessedMessage(
                messageId,
                messageType,
                timeProvider.GetUtcNow()));
    }

    private static OrderData Map(Order order) => new(
        order.PublicId,
        order.CustomerPublicId,
        order.Customer.FullName,
        order.Customer.Email,
        order.Customer.PhoneNumber,
        order.DeliveryAddress.Street,
        order.DeliveryAddress.HouseNumber,
        order.DeliveryAddress.Addition,
        order.DeliveryAddress.PostalCode,
        order.DeliveryAddress.City,
        order.DeliveryAddress.CountryCode,
        order.Status,
        order.OrderedAt,
        order.UpdatedAt,
        order.ShippedAt,
        order.TotalAmount,
        order.RejectionReason,
        order.Items.Select(item => new OrderItemData(
            item.ProductPublicId,
            item.ProductName,
            item.UnitPrice,
            item.Quantity,
            item.LineTotal)).ToList());
}
