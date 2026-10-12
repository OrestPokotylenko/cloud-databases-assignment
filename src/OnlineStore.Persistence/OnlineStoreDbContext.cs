using Microsoft.EntityFrameworkCore;

using OnlineStore.Domain.Products.Entities;
using OnlineStore.Domain.Orders.Entities;
using OnlineStore.Persistence.Messaging;

namespace OnlineStore.Persistence;

public class OnlineStoreDbContext(
    DbContextOptions<OnlineStoreDbContext> options)
    : DbContext(options)
{
    public DbSet<Product> Products => Set<Product>();
    public DbSet<Order> Orders => Set<Order>();
    public DbSet<OrderItem> OrderItems => Set<OrderItem>();
    public DbSet<ProcessedMessage> ProcessedMessages => Set<ProcessedMessage>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(OnlineStoreDbContext).Assembly);
    }
}
