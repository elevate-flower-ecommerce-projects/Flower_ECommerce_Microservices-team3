using Blocks.Contracts.Common;
using Blocks.Contracts.Pagination;
using Catalog_Service.Features.Products.GetProductsByOccasionId.ViewModels;
using MediatR;

namespace Catalog_Service.Features.Products.GetProductsByOccasionId.Queries
{
    public record GetProductsQuery(
    Guid OccasionId,
    int PageNumber = 1,
    int PageSize = 10)
    : IRequest<Result<PagedResult<ProductViewModel>>>;
}
