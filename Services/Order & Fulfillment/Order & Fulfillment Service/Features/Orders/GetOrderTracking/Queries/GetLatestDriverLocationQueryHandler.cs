using Blocks.Contracts.Common;
using Blocks.Contracts.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Order___Fulfillment_Service.Entities;
using Order___Fulfillment_Service.Features.Orders.GetOrderTracking.DTOs;

namespace Order___Fulfillment_Service.Features.Orders.GetOrderTracking.Queries
{
    public sealed class GetLatestDriverLocationQueryHandler(
     IGenericRepository<DriverLocation> driverLocationRepository)
     : IRequestHandler<GetLatestDriverLocationQuery, Result<LocationPointDto?>>
    {
        public async Task<Result<LocationPointDto?>> Handle(
            GetLatestDriverLocationQuery request,
            CancellationToken cancellationToken)
        {
            var location = await driverLocationRepository.GetQueryable()
                .AsNoTracking()
                .Where(dl => dl.DriverId == request.DriverId && dl.ActiveOrderId == request.OrderId)
                .OrderByDescending(dl => dl.RecordedAt)
                .Select(dl => new { dl.Lat, dl.Lng, dl.RecordedAt })
                .FirstOrDefaultAsync(cancellationToken);
            if (location is null)
            {
                return Result.Success<LocationPointDto?>(null);
            }
            var recordedAtUtc = DateTime.SpecifyKind(location.RecordedAt, DateTimeKind.Utc);
            bool isStale = (DateTime.UtcNow - recordedAtUtc) > TimeSpan.FromMinutes(10);

            return Result.Success<LocationPointDto?>(
                new LocationPointDto(location.Lat, location.Lng, recordedAtUtc, isStale));

        }
    }
}
