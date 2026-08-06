using Identity.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Identity.Infrastructure.Persistence.Configurations
{
    public class RefreshTokenConfiguration :IEntityTypeConfiguration<RefreshToken>
    {
        public void Configure(EntityTypeBuilder<RefreshToken> builder)
        {
            builder.Property(t => t.TokenHash)
                .IsRequired()
                .HasMaxLength(512);
            builder.Property(t => t.DeviceInfo)
                .HasMaxLength(500);
            builder.HasIndex(t => t.UserId);
            builder.HasIndex(t => t.TokenHash).IsUnique();
            builder.HasIndex(t => t.FamilyId);
        }
    }
}
