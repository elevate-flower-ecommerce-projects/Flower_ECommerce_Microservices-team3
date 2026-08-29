using Blocks.Contracts.Common;
using Blocks.Contracts.Pagination;
using Catalog_Service.Features.Products.Queries.GetProducts;
using MediatR;

namespace Catalog_Service.Features.Products.Queries.GetProductsPage
{
    public sealed record GetProductsPageQuery(
        int Page,
        int PageSize,
        Guid? CategoryId,
        Guid? OccasionId,
        Guid? StoreId
    ) : IRequest<Result<PagedResult<ProductSummaryResponse>>>;
}
