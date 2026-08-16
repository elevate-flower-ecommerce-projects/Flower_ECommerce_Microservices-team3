using Blocks.Contracts.Common;
using Blocks.Contracts.Pagination;
using Catalog_Service.Features.Products.ViewModels;
using MediatR;

namespace Catalog_Service.Features.Products.Queries
{
    public record GetProductsQuery(
    int PageNumber = 1,
    int PageSize = 10,
    Guid? OccasionId = null)
    : IRequest<Result<PagedResult<ProductViewModel>>>;
}
