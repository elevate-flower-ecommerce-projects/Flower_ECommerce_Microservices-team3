using Identity.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Text;

namespace Identity.Infrastructure.Persistence.Configurations
{
    public class DriverApplicationConfiguration : IEntityTypeConfiguration<DriverApplication>
    {
        public void Configure(EntityTypeBuilder<DriverApplication> builder)
        {
            builder.HasKey(x => x.Id);

            builder.HasOne(x => x.User)
                   .WithMany()
                   .HasForeignKey(x => x.UserId)
                   .IsRequired(false)
                   .OnDelete(DeleteBehavior.SetNull);

            builder.Property(x => x.VehicleNumber).HasMaxLength(50);
            builder.Property(x => x.NationalIdNumber).HasMaxLength(20);
            builder.Property(x => x.RejectionReason).HasMaxLength(500);
        }
    }
}
