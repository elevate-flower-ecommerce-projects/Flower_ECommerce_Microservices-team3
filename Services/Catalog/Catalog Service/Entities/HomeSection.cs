using Blocks.Domain.Entities;

namespace Catalog_Service.Entities;

public class HomeSection : AuditEntity
{
    public HomeSectionType Type { get; set; }
    public string Title { get; set; } = string.Empty;
    public string? TitleAr { get; set; }
    public int Index { get; set; }
    public Guid? OccasionId { get; set; }
    public Guid? CategoryId { get; set; }

    public Occasion? Occasion { get; set; }
    public Category? Category { get; set; }
}
