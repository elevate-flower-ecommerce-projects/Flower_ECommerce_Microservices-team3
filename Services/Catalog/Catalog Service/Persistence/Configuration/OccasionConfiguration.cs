using Catalog_Service.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Catalog_Service.Persistence.Configuration;

public class OccasionConfiguration : IEntityTypeConfiguration<Occasion>
{
    public void Configure(EntityTypeBuilder<Occasion> builder)
    {
        builder.ToTable("Occasions", "catalog");

        builder.HasKey(o => o.Id);

        builder.Property(o => o.Name)
            .IsRequired()
            .HasMaxLength(200);

        builder.Property(o => o.NameAr)
            .HasMaxLength(200);

        builder.Property(o => o.ImageUrl)
            .IsRequired()
            .HasMaxLength(500);
       
        builder.HasQueryFilter(o => o.IsActive);
    }
}
