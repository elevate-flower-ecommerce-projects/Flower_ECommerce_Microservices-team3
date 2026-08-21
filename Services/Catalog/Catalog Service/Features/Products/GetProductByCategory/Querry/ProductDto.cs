namespace Catalog_Service.Features.Products.GetProductByCategory.Querry;

public sealed class ProductDto
{
    public Guid Id { get; set; }

    public string Name { get; set; } = string.Empty;

    public string? ImageUrl { get; set; }

    public string Currency { get; set; } = string.Empty;

    public decimal Price { get; set; }

    public decimal? OriginalPrice { get; set; }

    public decimal? DiscountPercentage { get; set; }

    public string Status { get; set; } = string.Empty;

    public Guid CategoryId { get; set; }
}