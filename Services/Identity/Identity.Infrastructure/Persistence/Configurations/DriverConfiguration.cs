using Identity.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Text;

namespace Identity.Infrastructure.Persistence.Configurations
{
    public class DriverConfiguration : IEntityTypeConfiguration<Driver>
    {
        public void Configure(EntityTypeBuilder<Driver> builder)
        {
            builder.HasKey(x => x.Id);

            builder.HasOne(d => d.User)
                .WithOne()
                .HasForeignKey<Driver>(d => d.UserId)
                .OnDelete(DeleteBehavior.Cascade);


            builder.HasOne(x => x.DriverApplication)
                   .WithOne()
                   .HasForeignKey<Driver>(x => x.DriverApplicationId)
                   .OnDelete(DeleteBehavior.Restrict);

            builder.Property(x => x.VehicleNumber).HasMaxLength(50);
            builder.Property(x => x.NationalIdNumber).HasMaxLength(20);
        }
    }
}
