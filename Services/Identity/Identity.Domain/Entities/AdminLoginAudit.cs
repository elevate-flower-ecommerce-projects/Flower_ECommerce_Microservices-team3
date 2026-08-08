using Blocks.Domain.Entities;
using Identity.Domain.Enums;

namespace Identity.Domain.Entities;

public class AdminLoginAudit : BaseEntity
{
    public string Email { get; set; } = string.Empty;
    public string IpAddress { get; set; } = string.Empty;
    public string UserAgent { get; set; } = string.Empty;
    public AdminLoginOutcome Outcome { get; set; }
    public DateTime Timestamp { get; set; } = DateTime.UtcNow;
}
