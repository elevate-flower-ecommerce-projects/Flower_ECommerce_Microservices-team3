namespace Catalog_Service.Features.Occasions.GetPaginatedOccasions.ViewModels
{
    public record OccasionViewModel(
    Guid Id,
    string Name,
    string ImageUrl
    );
}
