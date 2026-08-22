namespace Catalog_Service.Features.Products.GetProductById;

public sealed record ProductDetailsDto
{
    public Guid Id { get; init; }
    public string Name { get; init; } = string.Empty;
    public string ImageUrl { get; init; } = string.Empty;
    public string Currency { get; init; } = string.Empty;
    public decimal Price { get; init; }
    public decimal? OriginalPrice { get; init; }
    public int? DiscountPercentage { get; init; }
    public string Status { get; init; } = string.Empty;
    public List<string> Images { get; init; } = [];
    public string Description { get; init; } = string.Empty;
    public List<ProductIncludeDto> Includes { get; init; } = [];
    public Guid? CategoryId { get; init; }
    public List<Guid> OccasionIds { get; init; } = [];
}

public sealed record ProductIncludeDto(string Name);
