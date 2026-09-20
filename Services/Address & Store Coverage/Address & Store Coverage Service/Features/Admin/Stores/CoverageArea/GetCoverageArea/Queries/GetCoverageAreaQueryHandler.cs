using Address___Store_Coverage_Service.Entities;
using Address___Store_Coverage_Service.Features.Admin.Stores.Common.DTOs;
using Address___Store_Coverage_Service.Features.Admin.Stores.CoverageArea.Common.DTOs;
using Blocks.Contracts.Common;
using Blocks.Contracts.Interfaces;
using Blocks.Domain.Errors;
using MediatR;
using Microsoft.EntityFrameworkCore;

using CoverageAreaEntity = Address___Store_Coverage_Service.Entities.CoverageArea;

namespace Address___Store_Coverage_Service.Features.Admin.Stores.CoverageArea.GetCoverageArea.Queries
{
    public sealed class GetCoverageAreaQueryHandler(
        IGenericRepository<CoverageAreaEntity> coverageAreaRepository)
        : IRequestHandler<GetCoverageAreaQuery, Result<CoverageAreaDto>>
    {
        public async Task<Result<CoverageAreaDto>> Handle(
            GetCoverageAreaQuery request,
            CancellationToken cancellationToken)
        {
            var coverageArea = await coverageAreaRepository.GetQueryable()
                .AsNoTracking()
                .FirstOrDefaultAsync(c => c.StoreId == request.StoreId && c.DeletedAt == null, cancellationToken);

            if (coverageArea is null)
            {
                return Result.Failure<CoverageAreaDto>(Error.NotFound("Coverage area for the specified store not found."));
            }

            var dto = new CoverageAreaDto(
                coverageArea.StoreId,
                coverageArea.BoundaryType,
                coverageArea.Polygon?.Select(p => new GeoLocationDto(p.Lat, p.Lng)).ToList(),
                coverageArea.RadiusMeters,
                coverageArea.Cities,
                coverageArea.Areas,
                coverageArea.UpdatedAt ?? coverageArea.CreatedAt);

            return Result.Success(dto);
        }
    }
}
