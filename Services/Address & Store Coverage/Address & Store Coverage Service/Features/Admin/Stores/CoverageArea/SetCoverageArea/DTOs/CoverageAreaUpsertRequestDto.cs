using Address___Store_Coverage_Service.Entities;
using Address___Store_Coverage_Service.Features.Admin.Stores.Common.DTOs;

namespace Address___Store_Coverage_Service.Features.Admin.Stores.CoverageArea.SetCoverageArea.DTOs
{
    public sealed record CoverageAreaUpsertRequestDto(
        CoverageBoundaryType BoundaryType,
        List<GeoLocationDto>? Polygon,
        double? RadiusMeters,
        List<string>? Cities,
        List<string>? Areas);
}
