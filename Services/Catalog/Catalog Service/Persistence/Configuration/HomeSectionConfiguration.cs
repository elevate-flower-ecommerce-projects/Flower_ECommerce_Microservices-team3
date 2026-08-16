using Catalog_Service.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Catalog_Service.Persistence.Configuration;

public class HomeSectionConfiguration : IEntityTypeConfiguration<HomeSection>
{
    public void Configure(EntityTypeBuilder<HomeSection> builder)
    {
        builder.HasKey(e => e.Id);

        builder.Property(e => e.Title)
            .HasMaxLength(200)
            .IsRequired();

        builder.Property(e => e.TitleAr)
            .HasMaxLength(200);

        builder.Property(e => e.Type)
            .HasConversion<string>()
            .HasMaxLength(50)
            .IsRequired();

        builder.HasIndex(e => e.Index);

        builder.HasOne(e => e.Occasion)
            .WithMany()
            .HasForeignKey(e => e.OccasionId)
            .OnDelete(DeleteBehavior.SetNull);

        builder.HasOne(e => e.Category)
            .WithMany()
            .HasForeignKey(e => e.CategoryId)
            .OnDelete(DeleteBehavior.SetNull);

        builder.HasQueryFilter(e => e.DeletedAt == null);
    }
}
