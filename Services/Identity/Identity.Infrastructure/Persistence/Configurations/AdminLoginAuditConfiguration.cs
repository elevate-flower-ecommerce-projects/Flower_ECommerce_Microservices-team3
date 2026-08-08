using Identity.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Identity.Infrastructure.Persistence.Configurations
{
    public class AdminLoginAuditConfiguration : IEntityTypeConfiguration<AdminLoginAudit>
    {
        public void Configure(EntityTypeBuilder<AdminLoginAudit> builder)
        {
            builder.Property(a => a.Email).IsRequired().HasMaxLength(256);
            builder.Property(a => a.IpAddress).IsRequired().HasMaxLength(45);
            builder.Property(a => a.UserAgent).HasMaxLength(512);

            builder.HasIndex(a => a.Email);
            builder.HasIndex(a => a.IpAddress);
            builder.HasIndex(a => a.Timestamp);
        }
    }
}
