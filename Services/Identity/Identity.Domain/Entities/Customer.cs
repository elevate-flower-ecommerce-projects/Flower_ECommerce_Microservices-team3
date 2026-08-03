using Blocks.Domain.Entities;

namespace Identity.Domain.Entities;

public class Customer : AuditEntity
{
    public Guid UserId { get; set; }
    public User User { get; set; } = null!;
}
