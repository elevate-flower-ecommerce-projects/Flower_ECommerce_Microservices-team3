using Catalog_Service.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Catalog_Service.Persistence.Configuration
{
    public sealed class InventoryConfiguration
        : IEntityTypeConfiguration<Inventory>
    {
        public void Configure(EntityTypeBuilder<Inventory> builder)
        {
            builder.ToTable("Inventories");

            builder.HasKey(i => i.Id);

            builder.Property(i => i.ProductId)
                .IsRequired();

            builder.Property(i => i.StoreId)
                .IsRequired();

            builder.Property(i => i.Quantity)
                .IsRequired();

            builder.HasIndex(i => new
            {
                i.ProductId,
                i.StoreId
            })
            .IsUnique();
        }
    }
}
