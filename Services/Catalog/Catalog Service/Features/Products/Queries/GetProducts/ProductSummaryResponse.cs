namespace Catalog_Service.Features.Products.Queries.GetProducts
{
    public sealed record ProductSummaryResponse(
        Guid Id,
        string Name,
        string? ImageUrl,
        decimal Price,
        decimal? DiscountedPrice,
        decimal? DiscountPercent,
        bool InStock
    );
}
