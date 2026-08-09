using System;
using System.Collections.Generic;
using System.Text;
using Blocks.Domain.Entities;
using Identity.Domain.Enums;

namespace Identity.Domain.Entities
{
    public class DriverApplication : AuditEntity
    {
        public DriverApplicationStatus Status { get; private set; }
        public string? RejectionReason { get; private set; }
        public Guid? ReviewedBy { get; private set; }
        public DateTime? ReviewedAt { get; private set; }


        // Required by EF Core
        private DriverApplication()
        {
        }

        #region relationship (1-1)
        public Guid UserId { get; set; }
        public User User { get; set; } = null!;
        #endregion
        public DriverApplication(Guid userId)
        {
            UserId = userId;
            Status = DriverApplicationStatus.Pending;
        }

        public void Approve(Guid adminId)
        {
            if (Status != DriverApplicationStatus.Pending)
                throw new InvalidOperationException(
                    "Only pending applications can be approved.");

            Status = DriverApplicationStatus.Approved;
            ReviewedBy = adminId;
            ReviewedAt = DateTime.UtcNow;
        }

        public void Reject(Guid adminId, string reason)
        {
            if (Status != DriverApplicationStatus.Pending)
                throw new InvalidOperationException(
                    "Only pending applications can be rejected.");

            Status = DriverApplicationStatus.Rejected;
            RejectionReason = reason;
            ReviewedBy = adminId;
            ReviewedAt = DateTime.UtcNow;
        }
    }
}
