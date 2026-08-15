using Blocks.Domain.Entities;

namespace Catalog_Service.Entities;

public class Occasion : AuditEntity
{
    public string Name { get; set; } = string.Empty;
    public string? NameAr { get; set; }

    public string ImageUrl { get; set; } = string.Empty;

    public ICollection<ProductOccasion> ProductOccasions { get; set; } = [];
}
