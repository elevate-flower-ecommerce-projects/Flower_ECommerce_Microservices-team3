using Catalog_Service.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Catalog_Service.Persistence.Configuration;

public class ProductOccasionConfiguration : IEntityTypeConfiguration<ProductOccasion>
{
    public void Configure(EntityTypeBuilder<ProductOccasion> builder)
    {
        builder.ToTable("ProductOccasions", "catalog");

        builder.HasKey(po => new { po.ProductId, po.OccasionId });

        builder.HasOne(po => po.Product)
            .WithMany(p => p.ProductOccasions)
            .HasForeignKey(po => po.ProductId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(po => po.Occasion)
            .WithMany(o => o.ProductOccasions)
            .HasForeignKey(po => po.OccasionId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
