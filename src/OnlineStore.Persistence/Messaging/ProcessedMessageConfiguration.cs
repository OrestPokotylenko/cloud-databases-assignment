using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace OnlineStore.Persistence.Messaging;

public class ProcessedMessageConfiguration : IEntityTypeConfiguration<ProcessedMessage>
{
    public void Configure(EntityTypeBuilder<ProcessedMessage> builder)
    {
        builder.HasKey(e => e.Id);

        builder.Property(e => e.Id)
            .ValueGeneratedOnAdd();

        builder.Property(e => e.MessageId)
            .IsRequired();

        builder.Property(e => e.MessageType)
            .IsRequired()
            .HasMaxLength(200);

        builder.Property(e => e.ProcessedAt)
            .IsRequired();

        builder.HasIndex(e => e.MessageId)
            .IsUnique();
    }
}
