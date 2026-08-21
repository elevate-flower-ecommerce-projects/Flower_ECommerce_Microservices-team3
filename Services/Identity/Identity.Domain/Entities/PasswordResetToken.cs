using System;
using System.Collections.Generic;
using System.Text;
using Blocks.Domain.Entities;

namespace Identity.Domain.Entities
{
    public class PasswordResetToken : AuditEntity
    {
        public string TokenHash { get; private set; } = null!;
        public DateTime ExpiresAt { get; private set; }
        public DateTime? UsedAt { get; private set; }

        #region relationship (1 - M)
        public Guid UserId { get; private set; }
        public User User { get; private set; } = null!;
        #endregion

        private PasswordResetToken()
        {
        }

        public PasswordResetToken(
            Guid userId,
            string tokenHash,
            DateTime createdAt)
        {
            UserId = userId;
            TokenHash = tokenHash;

            CreatedAt = createdAt;

            ExpiresAt = createdAt.AddMinutes(10);
        }

        public bool IsExpired(DateTime now)
        {
            return now >= ExpiresAt;
        }

        public bool IsUsed()
        {
            return UsedAt.HasValue;
        }

        public void MarkAsUsed(DateTime now)
        {
            UsedAt = now;
        }
    }
}
