using Blocks.Domain.Entities;

namespace Catalog_Service.Entities;

public class Product : AuditEntity
{
    public string Name { get; set; } = string.Empty;
    public string? NameAr { get; set; }
    public string ImageUrl { get; set; } = string.Empty;
    public string Currency { get; set; } = "EGP";
    public decimal Price { get; set; }
    public decimal? OriginalPrice { get; set; }
    public int? DiscountPercentage { get; set; }
    public ProductStatus Status { get; set; } = ProductStatus.InStock;
    public string? Description { get; set; }
    public string? DescriptionAr { get; set; }
    public bool IsBestSeller { get; set; }
    public int BestSellerOrder { get; set; }

    public Guid CategoryId { get; set; }
    public Category Category { get; set; } = null!;

    public ICollection<ProductImage> Images { get; set; } = [];
    public ICollection<ProductInclude> Includes { get; set; } = [];
    public ICollection<ProductOccasion> ProductOccasions { get; set; } = [];
}
