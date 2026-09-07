using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Order___Fulfillment_Service.Entities;

namespace Order___Fulfillment_Service.Persistence.Configurations;

public sealed class DriverLocationConfiguration : IEntityTypeConfiguration<DriverLocation>
{
    public void Configure(EntityTypeBuilder<DriverLocation> builder)
    {
        builder.ToTable("DriverLocations");

        builder.HasKey(dl => dl.Id);

        builder.Property(dl => dl.DriverId)
            .IsRequired();

        builder.Property(dl => dl.ActiveOrderId)
            .IsRequired(false);

        builder.Property(dl => dl.Lat)
            .IsRequired();

        builder.Property(dl => dl.Lng)
            .IsRequired();

        builder.Property(dl => dl.RecordedAt)
            .IsRequired();

        builder.HasIndex(dl => dl.DriverId)
            .IsUnique();

        builder.HasIndex(dl => dl.ActiveOrderId);
    }
}
