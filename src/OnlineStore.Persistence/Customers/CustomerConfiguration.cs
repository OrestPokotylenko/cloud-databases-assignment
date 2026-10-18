using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

using OnlineStore.Domain.Customers;
using OnlineStore.Domain.Customers.Entities;

namespace OnlineStore.Persistence.Customers;

public class CustomerConfiguration : IEntityTypeConfiguration<Customer>
{
    public void Configure(EntityTypeBuilder<Customer> builder)
    {
        builder.HasKey(e => e.Id);

        builder.Property(e => e.Id)
            .ValueGeneratedOnAdd();

        builder.Property(e => e.PublicId)
            .IsRequired()
            .HasDefaultValueSql("NEWID()")
            .ValueGeneratedOnAdd();

        builder.Property(e => e.FirstName)
            .IsRequired()
            .HasMaxLength(CustomerDetails.MaximumNameLength);

        builder.Property(e => e.LastName)
            .IsRequired()
            .HasMaxLength(CustomerDetails.MaximumNameLength);

        builder.Property(e => e.Email)
            .IsRequired()
            .HasMaxLength(CustomerDetails.MaximumEmailLength);

        builder.Property(e => e.CreatedAt)
            .IsRequired()
            .HasDefaultValueSql("SYSUTCDATETIME()")
            .ValueGeneratedOnAdd();

        builder.Property(e => e.UpdatedAt);

        builder.HasIndex(e => e.PublicId)
            .IsUnique();

        builder.HasIndex(e => e.Email)
            .IsUnique();

        builder.ToTable("Customers");
    }
}
