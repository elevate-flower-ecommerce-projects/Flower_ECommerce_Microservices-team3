using Address___Store_Coverage_Service.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System.Text.Json;

namespace Address___Store_Coverage_Service.Persistence.Configuration
{
    public sealed class CoverageAreaConfiguration : IEntityTypeConfiguration<CoverageArea>
    {
        private static readonly JsonSerializerOptions JsonOptions = new();

        public void Configure(EntityTypeBuilder<CoverageArea> builder)
        {
            builder.ToTable("CoverageAreas");

            builder.HasKey(c => c.Id);

            builder.Property(c => c.BoundaryType)
                .IsRequired()
                .HasConversion<string>()
                .HasMaxLength(50);

            builder.Property(c => c.RadiusMeters);

            var polygonComparer = new Microsoft.EntityFrameworkCore.ChangeTracking.ValueComparer<List<GeoPoint>?>(
                (c1, c2) => (c1 == null && c2 == null) || (c1 != null && c2 != null && c1.SequenceEqual(c2)),
                c => c == null ? 0 : c.Aggregate(0, (a, v) => HashCode.Combine(a, v.GetHashCode())),
                c => c == null ? null : c.ToList());

            var stringListComparer = new Microsoft.EntityFrameworkCore.ChangeTracking.ValueComparer<List<string>?>(
                (c1, c2) => (c1 == null && c2 == null) || (c1 != null && c2 != null && c1.SequenceEqual(c2)),
                c => c == null ? 0 : c.Aggregate(0, (a, v) => HashCode.Combine(a, v.GetHashCode())),
                c => c == null ? null : c.ToList());

            builder.Property(c => c.Polygon)
                .HasConversion(
                    v => v == null ? null : JsonSerializer.Serialize(v, JsonOptions),
                    v => string.IsNullOrEmpty(v) ? null : JsonSerializer.Deserialize<List<GeoPoint>>(v, JsonOptions),
                    polygonComparer);

            builder.Property(c => c.Cities)
                .HasConversion(
                    v => v == null ? null : JsonSerializer.Serialize(v, JsonOptions),
                    v => string.IsNullOrEmpty(v) ? null : JsonSerializer.Deserialize<List<string>>(v, JsonOptions),
                    stringListComparer);

            builder.Property(c => c.Areas)
                .HasConversion(
                    v => v == null ? null : JsonSerializer.Serialize(v, JsonOptions),
                    v => string.IsNullOrEmpty(v) ? null : JsonSerializer.Deserialize<List<string>>(v, JsonOptions),
                    stringListComparer);

            builder.HasIndex(c => c.StoreId)
                .IsUnique();
        }
    }
}
