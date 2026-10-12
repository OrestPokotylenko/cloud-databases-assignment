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

        builder.Property(e => e.CustomerPublicId)
            .IsRequired();

        builder.Property(e => e.Status)
            .HasConversion<string>()
            .HasMaxLength(20)
            .IsRequired();

        builder.Property(e => e.OrderedAt)
            .IsRequired();

        builder.Property(e => e.UpdatedAt)
            .IsRequired();

        builder.Property(e => e.ShippedAt);

        builder.Property(e => e.TotalAmount)
            .HasPrecision(18, 2)
            .IsRequired();

        builder.Property(e => e.RejectionReason)
            .HasMaxLength(500);

        builder.OwnsOne(e => e.Customer, customer =>
        {
            customer.Property(e => e.FullName)
                .HasColumnName("CustomerFullName")
                .HasMaxLength(200)
                .IsRequired();
            customer.Property(e => e.Email)
                .HasColumnName("CustomerEmail")
                .HasMaxLength(320)
                .IsRequired();
            customer.Property(e => e.PhoneNumber)
                .HasColumnName("CustomerPhoneNumber")
                .HasMaxLength(30);
        });

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

        builder.HasIndex(e => new { e.OrderedAt, e.Id })
            .IsDescending();

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
                "CK_Orders_ShippedAt",
                "[ShippedAt] IS NULL OR [ShippedAt] >= [OrderedAt]");
        });
    }
}
