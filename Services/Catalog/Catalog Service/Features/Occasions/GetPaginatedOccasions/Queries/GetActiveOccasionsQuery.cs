using Blocks.Contracts.Common;
using Blocks.Contracts.Pagination;
using Catalog_Service.Features.Occasions.GetPaginatedOccasions.ViewModels;
using MediatR;

namespace Catalog_Service.Features.Occasions.GetPaginatedOccasions.Queries
{
    public record GetActiveOccasionsQuery(int PageNumber = 1, int PageSize = 10)
     : IRequest<Result<PagedResult<OccasionViewModel>>>;
}
