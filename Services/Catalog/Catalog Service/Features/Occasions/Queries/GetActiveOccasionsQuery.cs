using Blocks.Contracts.Common;
using Catalog_Service.Features.Occasions.ViewModels;
using MediatR;

namespace Catalog_Service.Features.Occasions.Queries
{
    public record GetActiveOccasionsQuery : IRequest<Result<IReadOnlyList<OccasionViewModel>>>;
}
