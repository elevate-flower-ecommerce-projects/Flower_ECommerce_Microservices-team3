using Blocks.Domain.Entities;
using Identity.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Text;

namespace Identity.Domain.Entities
{
    public class DriverApplication : AuditEntity
    {
        public Guid? UserId { get; set; }
        public User User { get; set; } = null!;

        public VehicleType VehicleType { get; set; }
        public string VehicleNumber { get; set; } = string.Empty;
        public string VehicleLicenceImage { get; set; } = string.Empty;
        public string NationalIdNumber { get; set; } = string.Empty;
        public string NationalIdImage { get; set; } = string.Empty;

        public DriverApplicationStatus Status { get; private set; } = DriverApplicationStatus.Pending;
        public string? RejectionReason { get; private set; }

        public Guid? ReviewedBy { get; private set; }
        public DateTime? ReviewedAt { get; private set; }


        public void Approve(Guid adminId)
        {
            if (Status != DriverApplicationStatus.Pending)
            {
                throw new InvalidOperationException("Application is already decided.");
            }

            Status = DriverApplicationStatus.Approved;
            ReviewedBy = adminId;
            ReviewedAt = DateTime.UtcNow;
        }

        public void Reject(string reason, Guid adminId)
        {
            if (Status != DriverApplicationStatus.Pending)
            {
                throw new InvalidOperationException("Application is already decided.");
            }

            if (string.IsNullOrWhiteSpace(reason))
            {
                throw new ArgumentException("Rejection reason is required.");
            }

            Status = DriverApplicationStatus.Rejected;
            RejectionReason = reason;
            ReviewedBy = adminId;
            ReviewedAt = DateTime.UtcNow;
        }
    }
}
