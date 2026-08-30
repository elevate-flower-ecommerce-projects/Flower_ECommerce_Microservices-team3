namespace Catalog_Service.Features.Products.Queries.GetActivePromotions
{
    public sealed record ProductPromotionResponse(
    Guid ProductId,
    decimal DiscountPercent);
}
