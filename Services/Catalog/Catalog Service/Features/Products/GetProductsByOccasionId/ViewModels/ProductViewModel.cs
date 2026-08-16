namespace Catalog_Service.Features.Products.GetProductsByOccasionId.ViewModels
{
    public record ProductViewModel(
    Guid Id,
    string Name,
    string ImageUrl,
    string Currency,
    decimal Price,
    decimal? OriginalPrice,
    int? DiscountPercentage,
    string Status,
    bool IsBestSeller
    );
}
