using Address___Store_Coverage_Service.Features.Cities.DTOs;
using Blocks.Contracts.Common;
using MediatR;

namespace Address___Store_Coverage_Service.Features.Cities.Queries
{
    public sealed record GetCitiesWithAreasQuery : IRequest<Result<List<CityWithAreasDto>>>;
}
