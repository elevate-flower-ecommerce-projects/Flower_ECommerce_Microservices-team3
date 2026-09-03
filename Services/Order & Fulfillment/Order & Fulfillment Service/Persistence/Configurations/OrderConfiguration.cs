using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Order___Fulfillment_Service.Entities;

namespace Order___Fulfillment_Service.Persistence.Configurations;

public sealed class OrderConfiguration : IEntityTypeConfiguration<Order>
{
    public void Configure(EntityTypeBuilder<Order> builder)
    {
        builder.ToTable("Orders");

        builder.HasKey(o => o.Id);

        builder.Property(o => o.Status)
            .HasConversion<string>()
            .HasMaxLength(50)
            .IsRequired();

        builder.Property(o => o.PaymentMethod)
            .HasConversion<string>()
            .HasMaxLength(50)
            .IsRequired();

        builder.Property(o => o.PaymentGateway)
            .HasConversion<string>()
            .HasMaxLength(50)
            .IsRequired(false);

        builder.Property(o => o.Subtotal)
            .HasPrecision(18, 2)
            .IsRequired();

        builder.Property(o => o.DeliveryFee)
            .HasPrecision(18, 2)
            .IsRequired();

        builder.Property(o => o.Total)
            .HasPrecision(18, 2)
            .IsRequired();

        builder.Property(o => o.RecipientName)
            .HasMaxLength(150)
            .IsRequired();

        builder.Property(o => o.RecipientPhone)
            .HasMaxLength(20)
            .IsRequired();

        builder.Property(o => o.AddressLine)
            .HasMaxLength(500)
            .IsRequired();

        builder.Property(o => o.City)
            .HasMaxLength(100)
            .IsRequired();

        builder.Property(o => o.Area)
            .HasMaxLength(100)
            .IsRequired();

        builder.Property(o => o.GiftRecipientName)
            .HasMaxLength(150)
            .IsRequired(false);

        builder.Property(o => o.GiftRecipientPhone)
            .HasMaxLength(20)
            .IsRequired(false);

        builder.Property(o => o.CancellationReason)
            .HasMaxLength(500)
            .IsRequired(false);

        builder.HasMany(o => o.Items)
            .WithOne(i => i.Order)
            .HasForeignKey(i => i.OrderId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasIndex(o => o.CustomerId);
        builder.HasIndex(o => o.CartId);
        builder.HasIndex(o => o.Status);
        builder.HasIndex(o => o.CreatedAt);
    }
}
