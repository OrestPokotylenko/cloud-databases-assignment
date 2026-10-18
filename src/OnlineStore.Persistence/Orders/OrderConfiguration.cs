using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

using OnlineStore.Domain.Orders.Entities;

namespace OnlineStore.Persistence.Orders;

public class OrderConfiguration : IEntityTypeConfiguration<Order>
{
    public void Configure(EntityTypeBuilder<Order> builder)
    {
        builder.HasKey(e => e.Id);

        builder.Property(e => e.Id)
            .ValueGeneratedOnAdd();

        builder.Property(e => e.PublicId)
            .IsRequired()
            .ValueGeneratedNever();

        builder.Property(e => e.CustomerId)
            .IsRequired();

        builder.Property(e => e.Status)
            .HasConversion<string>()
            .HasMaxLength(20)
            .IsRequired();

        builder.Property(e => e.OrderDate)
            .IsRequired();

        builder.Property(e => e.UpdatedAt)
            .IsRequired();

        builder.Property(e => e.ShippingDate);

        builder.Ignore(e => e.ProcessingDuration);

        builder.Property(e => e.TotalAmount)
            .HasPrecision(18, 2)
            .IsRequired();

        builder.Property(e => e.RejectionReason)
            .HasMaxLength(500);

        builder.OwnsOne(e => e.DeliveryAddress, address =>
        {
            address.Property(e => e.Street)
                .HasColumnName("DeliveryStreet")
                .HasMaxLength(200)
                .IsRequired();
            address.Property(e => e.HouseNumber)
                .HasColumnName("DeliveryHouseNumber")
                .HasMaxLength(20)
                .IsRequired();
            address.Property(e => e.Addition)
                .HasColumnName("DeliveryAddressAddition")
                .HasMaxLength(20);
            address.Property(e => e.PostalCode)
                .HasColumnName("DeliveryPostalCode")
                .HasMaxLength(20)
                .IsRequired();
            address.Property(e => e.City)
                .HasColumnName("DeliveryCity")
                .HasMaxLength(100)
                .IsRequired();
            address.Property(e => e.CountryCode)
                .HasColumnName("DeliveryCountryCode")
                .HasMaxLength(2)
                .IsFixedLength()
                .IsRequired();
        });

        builder.HasIndex(e => e.PublicId)
            .IsUnique();

        builder.HasIndex(e => new { e.OrderDate, e.Id })
            .IsDescending();

        builder.HasOne(e => e.Customer)
            .WithMany()
            .HasForeignKey(e => e.CustomerId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasMany(e => e.Items)
            .WithOne()
            .HasForeignKey(e => e.OrderId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.ToTable("Orders", tableBuilder =>
        {
            tableBuilder.HasCheckConstraint(
                "CK_Orders_TotalAmount_NonNegative",
                "[TotalAmount] >= 0");
            tableBuilder.HasCheckConstraint(
                "CK_Orders_ShippingDate",
                "[ShippingDate] IS NULL OR [ShippingDate] >= [OrderDate]");
        });
    }
}
