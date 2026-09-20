using Address___Store_Coverage_Service.Entities;
using Address___Store_Coverage_Service.Features.Admin.Stores.Common.DTOs;
using Address___Store_Coverage_Service.Features.Admin.Stores.CoverageArea.Common.DTOs;
using Blocks.Contracts.Common;
using MediatR;

namespace Address___Store_Coverage_Service.Features.Admin.Stores.CoverageArea.SetCoverageArea.Commands
{
    public sealed record SetCoverageAreaCommand(
        Guid StoreId,
        CoverageBoundaryType BoundaryType,
        List<GeoLocationDto>? Polygon,
        double? RadiusMeters,
        List<string>? Cities,
        List<string>? Areas) : IRequest<Result<CoverageAreaDto>>;
}
