using Catalog_Service.Entities.Enums;

namespace Catalog_Service.Features.Products.Queries.GetProducts
{
    public sealed record GetProductsRequest(
        int Page = 1,
        int PageSize = 20,
        Guid? CategoryId = null,
        Guid? OccasionId = null,
        Guid? StoreId = null,
        ProductSort? Sort = null
    );
}
