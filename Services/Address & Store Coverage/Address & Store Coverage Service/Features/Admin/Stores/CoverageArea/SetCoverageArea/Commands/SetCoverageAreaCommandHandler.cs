using Address___Store_Coverage_Service.Entities;
using Address___Store_Coverage_Service.Features.Admin.Stores.Common.DTOs;
using Address___Store_Coverage_Service.Features.Admin.Stores.CoverageArea.Common.DTOs;
using Address___Store_Coverage_Service.Features.Admin.Stores.CoverageArea.SetCoverageArea.Commands;
using Address___Store_Coverage_Service.Persistence;
using Blocks.Contracts.Common;
using Blocks.Contracts.Interfaces;
using Blocks.Domain.Errors;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Address___Store_Coverage_Service.Features.Admin.Stores.CoverageArea.SetCoverageArea.Commands
{
    public sealed class SetCoverageAreaCommandHandler(
        IGenericRepository<Store> storeRepository,
        IUnitOfWork unitOfWork)
        : IRequestHandler<SetCoverageAreaCommand, Result<CoverageAreaDto>>
    {
        public async Task<Result<CoverageAreaDto>> Handle(
            SetCoverageAreaCommand request,
            CancellationToken cancellationToken)
        {
            return await unitOfWork.ExecuteAsync(async () =>
            {
                var store = await storeRepository.GetQueryable()
                    .Include(s => s.CoverageArea)
                    .FirstOrDefaultAsync(s => s.Id == request.StoreId && s.DeletedAt == null, cancellationToken);

                if (store is null)
                {
                    return Result.Failure<CoverageAreaDto>(Error.NotFound("Store not found."));
                }

                var polygonEntities = request.BoundaryType == CoverageBoundaryType.Polygon
                    ? request.Polygon?.Select(p => new GeoPoint(p.Lat, p.Lng)).ToList()
                    : null;

                var radiusMeters = request.BoundaryType == CoverageBoundaryType.Radius
                    ? request.RadiusMeters
                    : null;

                var cities = request.BoundaryType == CoverageBoundaryType.CityAreaList
                    ? request.Cities
                    : null;

                var areas = request.BoundaryType == CoverageBoundaryType.CityAreaList
                    ? request.Areas
                    : null;

                if (store.CoverageArea is null)
                {
                    store.CoverageArea = new Entities.CoverageArea
                    {
                        StoreId = store.Id,
                        BoundaryType = request.BoundaryType,
                        RadiusMeters = radiusMeters,
                        Polygon = polygonEntities,
                        Cities = cities,
                        Areas = areas,
                        CreatedAt = DateTime.UtcNow,
                        UpdatedAt = DateTime.UtcNow
                    };
                }
                else
                {
                    store.CoverageArea.BoundaryType = request.BoundaryType;
                    store.CoverageArea.RadiusMeters = radiusMeters;
                    store.CoverageArea.Polygon = polygonEntities;
                    store.CoverageArea.Cities = cities;
                    store.CoverageArea.Areas = areas;
                    store.CoverageArea.UpdatedAt = DateTime.UtcNow;
                }

                if (request.BoundaryType == CoverageBoundaryType.Radius && radiusMeters.HasValue)
                {
                    store.CoverageRadiusKm = radiusMeters.Value / 1000.0;
                }

                return Result.Success(new CoverageAreaDto(
                    store.CoverageArea.StoreId,
                    store.CoverageArea.BoundaryType,
                    store.CoverageArea.Polygon?.Select(p => new GeoLocationDto(p.Lat, p.Lng)).ToList(),
                    store.CoverageArea.RadiusMeters,
                    store.CoverageArea.Cities,
                    store.CoverageArea.Areas,
                    store.CoverageArea.UpdatedAt ?? store.CoverageArea.CreatedAt));
            }, cancellationToken);
        }
    }
}
