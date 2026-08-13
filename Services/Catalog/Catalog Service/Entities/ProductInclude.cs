using Blocks.Domain.Entities;

namespace Catalog_Service.Entities;

public class ProductInclude : BaseEntity
{
    public string Name { get; set; } = string.Empty;
    public string? NameAr { get; set; }

    public Guid ProductId { get; set; }
    public Product Product { get; set; } = null!;
}
