using Microsoft.EntityFrameworkCore;

using OnlineStore.Domain.Products.Entities;

namespace OnlineStore.Persistence;

public sealed class OnlineStoreDbContext(
    DbContextOptions<OnlineStoreDbContext> options)
    : DbContext(options)
{
    public DbSet<Product> Products => Set<Product>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(OnlineStoreDbContext).Assembly);
    }
}
