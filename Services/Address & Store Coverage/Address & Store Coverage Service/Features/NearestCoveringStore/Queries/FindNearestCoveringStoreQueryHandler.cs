using Address___Store_Coverage_Service.Entities;
using Address___Store_Coverage_Service.Features.NearestCoveringStore.DTOs;
using Blocks.Contracts.Common;
using Blocks.Contracts.Interfaces;
using Blocks.Domain.Errors;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Address___Store_Coverage_Service.Features.NearestCoveringStore.Queries
{
    public sealed class FindNearestCoveringStoreQueryHandler(
        IGenericRepository<Store> storeRepository)
        : IRequestHandler<FindNearestCoveringStoreQuery, Result<NearestStoreDto>>
    {
        private const double EarthRadiusKm = 6371.0;
        private const double MinKmPerDegreeLat = 110.574;
        public async Task<Result<NearestStoreDto>> Handle(
            FindNearestCoveringStoreQuery request,
            CancellationToken cancellationToken)
        {
            var candidates = await storeRepository.GetQueryable()
                .AsNoTracking()
                .Where(s => s.IsActive && s.DeletedAt == null)
                .Where(s => Math.Abs(s.Latitude - request.Latitude)
                            <= s.CoverageRadiusKm / MinKmPerDegreeLat)
                .Select(s => new { s.Id, s.Latitude, s.Longitude, s.CoverageRadiusKm })
                .ToListAsync(cancellationToken);
            var nearest = candidates
                .Select(s => new
                {
                    s.Id,
                    DistanceKm = CalculateDistanceKm(
                        request.Latitude, request.Longitude,
                        s.Latitude, s.Longitude),
                    s.CoverageRadiusKm
                })
                .Where(s => s.DistanceKm <= s.CoverageRadiusKm)
                .OrderBy(s => s.DistanceKm)
                .ThenBy(s => s.Id)
                .FirstOrDefault();
            if (nearest is null)
            {
                return Result.Failure<NearestStoreDto>(
                    Error.Validation("This location is outside our delivery coverage area."));
            }
            return Result.Success(new NearestStoreDto(nearest.Id));
        }
        private static double CalculateDistanceKm(
          double lat1, double lon1,
          double lat2, double lon2)
        {
            var dLat = ToRadians(lat2 - lat1);
            var dLon = ToRadians(lon2 - lon1);
            var a = Math.Sin(dLat / 2) * Math.Sin(dLat / 2)
                  + Math.Cos(ToRadians(lat1)) * Math.Cos(ToRadians(lat2))
                  * Math.Sin(dLon / 2) * Math.Sin(dLon / 2);
            var c = 2 * Math.Atan2(Math.Sqrt(a), Math.Sqrt(1 - a));
            return EarthRadiusKm * c;
        }
        private static double ToRadians(double degrees) => degrees * Math.PI / 180.0;
    }
}
