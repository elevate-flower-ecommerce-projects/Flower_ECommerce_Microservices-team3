using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Payment_Service.Entities;

public class PaymentWebhookEventConfiguration
    : IEntityTypeConfiguration<PaymentWebhookEvent>
{
    public void Configure(
        EntityTypeBuilder<PaymentWebhookEvent> builder)
    {
        builder.HasKey(x => x.Id);

        builder.HasIndex(x => x.EventId)
            .IsUnique();

        builder.Property(x => x.EventId)
            .IsRequired()
            .HasMaxLength(200);

        builder.Property(x => x.EventType)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(x => x.ReceivedAt)
            .IsRequired();
    }
}