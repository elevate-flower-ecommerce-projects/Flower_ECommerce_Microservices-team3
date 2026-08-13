using Blocks.Domain.Entities;

namespace Catalog_Service.Entities;

public class Category : AuditEntity
{
    public string Name { get; set; } = string.Empty;
    public string? NameAr { get; set; }

    public ICollection<Product> Products { get; set; } = [];
}
