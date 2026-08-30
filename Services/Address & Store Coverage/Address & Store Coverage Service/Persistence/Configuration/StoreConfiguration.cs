using Address___Store_Coverage_Service.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Address___Store_Coverage_Service.Persistence.Configuration
{
    public sealed class StoreConfiguration : IEntityTypeConfiguration<Store>
    {
        public void Configure(EntityTypeBuilder<Store> builder)
        {
            builder.ToTable("Stores");
            builder.HasKey(s => s.Id);
            builder.Property(s => s.Name)
                .IsRequired()
                .HasMaxLength(150);
            builder.Property(s => s.Latitude)
                .IsRequired();
            builder.Property(s => s.Longitude)
                .IsRequired();
            builder.Property(s => s.CoverageRadiusKm)
                .IsRequired();

            builder.HasOne(s => s.CoverageArea)
                .WithOne(c => c.Store)
                .HasForeignKey<CoverageArea>(c => c.StoreId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
