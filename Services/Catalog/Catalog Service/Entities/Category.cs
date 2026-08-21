using Blocks.Domain.Entities;

namespace Catalog_Service.Entities;

public class Category : AuditEntity
{
        public Guid Id { get; set; }

        public string Name { get; set; } = string.Empty;

        public string? Icon { get; set; }

        public bool IsActive { get; set; }

        public int DisplayOrder { get; set; }



    public ICollection<Product> Products { get; set; } = [];
}
