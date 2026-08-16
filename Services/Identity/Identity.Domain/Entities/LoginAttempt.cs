using Blocks.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace Identity.Domain.Entities
{
    public class LoginAttempt : BaseEntity
    {
        public string Email { get; set; } = string.Empty;
        public string IpAddress { get; set; } = string.Empty;
        public bool IsSuccessful { get; set; }
        public DateTime AttemptedAt { get; set; } = DateTime.UtcNow;
    }
}
