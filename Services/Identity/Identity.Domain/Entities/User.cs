using Blocks.Domain.Entities;
using Identity.Domain.Enums;

namespace Identity.Domain.Entities;

public class User : AuditEntity
{
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string HashPassword { get; set; } = string.Empty;
    public string Phone { get; set; } = string.Empty;
    public UserRole Role { get; set; }
    public Gender Gender { get; set; }
}
