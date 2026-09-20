using Address___Store_Coverage_Service.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Address___Store_Coverage_Service.Persistence.Configuration
{
    public sealed class AddressConfiguration : IEntityTypeConfiguration<Address>
    {
        public void Configure(EntityTypeBuilder<Address> builder)
        {
            builder.ToTable("Addresses");

            builder.HasKey(a => a.Id);

            builder.Property(a => a.RecipientName)
                .IsRequired()
                .HasMaxLength(100);

            builder.Property(a => a.Phone)
                .IsRequired()
                .HasMaxLength(20);

            builder.Property(a => a.AddressLine)
                .IsRequired()
                .HasMaxLength(250);

            builder.Property(a => a.Label)
                .HasMaxLength(50);

           
            builder.HasOne(a => a.City)
                .WithMany()
                .HasForeignKey(a => a.CityId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(a => a.Area)
                .WithMany()
                .HasForeignKey(a => a.AreaId)
                .OnDelete(DeleteBehavior.Restrict);

           
            builder.HasIndex(a => a.CustomerId, "IX_Addresses_CustomerId");

            
            builder.HasIndex(a => a.CustomerId, "UX_Addresses_CustomerId_IsDefault")
                .IsUnique()
                .HasFilter("[IsDefault] = 1 AND [DeletedAt] IS NULL");
        }
    }
}
