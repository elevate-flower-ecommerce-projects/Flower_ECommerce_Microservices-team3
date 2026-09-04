using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Order___Fulfillment_Service.Entities;

namespace Order___Fulfillment_Service.Persistence.Configurations;

public sealed class OrderItemConfiguration : IEntityTypeConfiguration<OrderItem>
{
    public void Configure(EntityTypeBuilder<OrderItem> builder)
    {
        builder.ToTable("OrderItems");

        builder.HasKey(i => i.Id);

        builder.Property(i => i.ProductName)
            .HasMaxLength(250)
            .IsRequired();

        builder.Property(i => i.ThumbnailUrl)
            .HasMaxLength(500)
            .IsRequired(false);

        builder.Property(i => i.UnitPrice)
            .HasPrecision(18, 2)
            .IsRequired();

        builder.Ignore(i => i.Subtotal);

        builder.HasIndex(i => i.OrderId);
        builder.HasIndex(i => i.ProductId);
    }
}
