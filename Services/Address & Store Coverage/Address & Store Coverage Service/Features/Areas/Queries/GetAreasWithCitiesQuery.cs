using Address___Store_Coverage_Service.Features.Areas.DTOs;
using Blocks.Contracts.Common;
using MediatR;

namespace Address___Store_Coverage_Service.Features.Areas.Queries
{
    public sealed record GetAreasWithCitiesQuery : IRequest<Result<List<AreaWithCitiesDto>>>;
}
