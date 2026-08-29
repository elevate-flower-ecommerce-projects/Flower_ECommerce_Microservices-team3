using Address___Store_Coverage_Service.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Address___Store_Coverage_Service.Persistence.Configuration
{
    public sealed class AreaConfiguration : IEntityTypeConfiguration<Area>
    {
        public void Configure(EntityTypeBuilder<Area> builder)
        {
            builder.ToTable("Areas");

            builder.HasKey(a => a.Id);

            builder.Property(a => a.Name)
                .IsRequired()
                .HasMaxLength(100);

            builder.HasIndex(a => a.Name)
                .IsUnique()
                .HasFilter("[DeletedAt] IS NULL");

            builder.HasMany(a => a.Cities)
                .WithOne(c => c.Area)
                .HasForeignKey(c => c.AreaId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
