using Catalog_Service.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Catalog_Service.Persistence.Configuration;

public class ProductIncludeConfiguration : IEntityTypeConfiguration<ProductInclude>
{
    public void Configure(EntityTypeBuilder<ProductInclude> builder)
    {
        builder.ToTable("ProductIncludes", "catalog");

        builder.HasKey(pi => pi.Id);

        builder.Property(pi => pi.Name)
            .IsRequired()
            .HasMaxLength(200);

        builder.Property(pi => pi.NameAr)
            .HasMaxLength(200);

        builder.HasOne(pi => pi.Product)
            .WithMany(p => p.Includes)
            .HasForeignKey(pi => pi.ProductId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
