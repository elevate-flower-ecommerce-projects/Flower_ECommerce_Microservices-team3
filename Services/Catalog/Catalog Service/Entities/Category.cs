using Blocks.Domain.Entities;

namespace Catalog_Service.Entities;

public class Category : AuditEntity
{
    public string Name { get; set; } = string.Empty;
    public string? NameAr { get; set; }
    public string? Icon { get; set; }
    public int DisplayOrder { get; set; }

    public ICollection<Product> Products { get; set; } = [];
}
