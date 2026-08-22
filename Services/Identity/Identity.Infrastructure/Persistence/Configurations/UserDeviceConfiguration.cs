using Identity.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Identity.Infrastructure.Persistence.Configurations
{
    public class UserDeviceConfiguration : IEntityTypeConfiguration<UserDevice>
    {
        public void Configure(EntityTypeBuilder<UserDevice> builder)
        {
            builder.ToTable("UserDevices");

            builder.HasKey(x => x.Id);

            builder.Property(x => x.DeviceId)
                .IsRequired()
                .HasMaxLength(128)
                .IsUnicode(false);

            builder.Property(x => x.FcmToken)
                .IsRequired()
                .HasMaxLength(512)
                .IsUnicode(false);

            builder.Property(x => x.UpdatedAt)
                .IsRequired();

            
            builder.HasIndex(x => new { x.UserId, x.DeviceId })
                .IsUnique()
                .HasDatabaseName("UX_UserDevices_UserId_DeviceId");

          
            builder.HasIndex(x => x.FcmToken)
                .IsUnique()
                .HasDatabaseName("UX_UserDevices_FcmToken");

            builder.HasIndex(x => new { x.UserId, x.UpdatedAt })
                .HasDatabaseName("IX_UserDevices_UserId_UpdatedAt");

            builder.HasOne(x => x.User)
                .WithMany()
                .HasForeignKey(x => x.UserId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
