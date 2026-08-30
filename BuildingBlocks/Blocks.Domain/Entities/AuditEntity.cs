namespace Blocks.Domain.Entities;

public abstract class AuditEntity : BaseEntity
{
    public DateTime CreatedAt { get; set; }
    public string? CreatedBy { get; set; }

    public DateTime? UpdatedAt { get; set; }
    public string? UpdatedBy { get; set; }

    public DateTime? DeletedAt { get; set; }
    public string? DeletedBy { get; set; }

    public bool IsActive { get; set; } = true;

    public void Activate()
    {
        IsActive = true;
        DeletedAt = null;
        DeletedBy = null;
    }

    public void Deactivate(string? deletedBy = null)
    {
        IsActive = false;
        DeletedAt = DateTime.UtcNow;
        DeletedBy = deletedBy;
    }
}
