using Blocks.Contracts.Common;
using Blocks.Contracts.Pagination;
using Catalog_Service.Entities.Enums;
using MediatR;

namespace Catalog_Service.Features.Products.Queries.GetProducts
{
    public sealed record GetProductsQuery(
        int Page = 1,
        int PageSize = 20,
        Guid? CategoryId = null,
        Guid? OccasionId = null,
        Guid? StoreId = null,
        ProductSort? Sort = null
    ) : IRequest<Result<PagedResult<ProductSummaryResponse>>>;
}
