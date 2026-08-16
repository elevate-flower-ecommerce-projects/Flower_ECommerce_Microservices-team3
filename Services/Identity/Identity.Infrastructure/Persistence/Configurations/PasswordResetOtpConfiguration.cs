using System;
using System.Collections.Generic;
using System.Text;
using Identity.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Identity.Infrastructure.Persistence.Configurations
{
    public class PasswordResetOtpConfiguration 
        : IEntityTypeConfiguration<PasswordResetOtp>
    {
        public void Configure(
            EntityTypeBuilder<PasswordResetOtp> builder)
        {
            builder.HasKey(x => x.Id);

            builder.Property(x => x.OtpHash)
                .IsRequired()
                .HasMaxLength(64);

            builder.Property(x => x.ExpiresAt)
                .IsRequired();

            builder.Property(x => x.LastSentAt)
                .IsRequired();

            builder.Property(x => x.AttemptsRemaining)
                .IsRequired();

            builder.Property(x => x.IsUsed)
                .IsRequired();

            builder.HasOne(x => x.User)
                .WithMany()
                .HasForeignKey(x => x.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasIndex(x => new
            {
                x.UserId,
                x.CreatedAt
            });
        }
    }
}
