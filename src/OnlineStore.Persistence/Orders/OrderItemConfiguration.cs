using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

using OnlineStore.Domain.Orders.Entities;

namespace OnlineStore.Persistence.Orders;

public class OrderItemConfiguration : IEntityTypeConfiguration<OrderItem>
{
    public void Configure(EntityTypeBuilder<OrderItem> builder)
    {
        builder.HasKey(e => e.Id);

        builder.Property(e => e.Id)
            .ValueGeneratedOnAdd();

        builder.Property(e => e.OrderId)
            .IsRequired();

        builder.Property(e => e.ProductPublicId)
            .IsRequired();

        builder.Property(e => e.ProductName)
            .HasMaxLength(200)
            .IsRequired();

        builder.Property(e => e.UnitPrice)
            .HasPrecision(18, 2)
            .IsRequired();

        builder.Property(e => e.Quantity)
            .IsRequired();

        builder.Ignore(e => e.LineTotal);

        builder.HasIndex(e => new { e.OrderId, e.ProductPublicId })
            .IsUnique();

        builder.ToTable("OrderItems", tableBuilder =>
        {
            tableBuilder.HasCheckConstraint(
                "CK_OrderItems_UnitPrice_NonNegative",
                "[UnitPrice] >= 0");
            tableBuilder.HasCheckConstraint(
                "CK_OrderItems_Quantity_Positive",
                "[Quantity] > 0");
        });
    }
}
