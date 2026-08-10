using Identity.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Text;

namespace Identity.Infrastructure.Persistence.Configurations
{
    public class LoginAttemptConfiguration : IEntityTypeConfiguration<LoginAttempt>
    {
        public void Configure(EntityTypeBuilder<LoginAttempt> builder) 
        {
            
            builder.HasKey(x => x.Id);
            builder.Property(x => x.Email)
                .IsRequired()
                .HasMaxLength(256);
            builder.Property(x => x.IpAddress)
                .IsRequired()
                .HasMaxLength(45);
            builder.HasIndex(x => x.Email);
            builder.HasIndex(x => x.IpAddress);
            builder.HasIndex(x => x.AttemptedAt);

            //builder.HasIndex(x => new { x.Email, x.IsSuccessful, x.AttemptedAt });


        }
    }
}
