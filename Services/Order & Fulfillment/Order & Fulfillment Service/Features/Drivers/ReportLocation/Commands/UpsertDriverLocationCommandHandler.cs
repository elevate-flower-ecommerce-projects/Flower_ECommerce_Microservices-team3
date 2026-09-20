using Blocks.Contracts.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Order___Fulfillment_Service.Entities;
using Order___Fulfillment_Service.Persistence;

namespace Order___Fulfillment_Service.Features.Drivers.ReportLocation.Commands
{
    public class UpsertDriverLocationCommandHandler(
    IGenericRepository<DriverLocation> locationRepository,
    IUnitOfWork unitOfWork)
    : IRequestHandler<UpsertDriverLocationCommand, bool>
    {
        public async Task<bool> Handle(
            UpsertDriverLocationCommand request,
            CancellationToken cancellationToken)
        {
            var existingLocation = await locationRepository.GetQueryable()
                .FirstOrDefaultAsync(l => l.DriverId == request.DriverId, cancellationToken);
            if (existingLocation != null)
            {
                existingLocation.ActiveOrderId = request.ActiveOrderId;
                existingLocation.Lat = request.Lat;
                existingLocation.Lng = request.Lng;
                existingLocation.RecordedAt = request.RecordedAt;
                locationRepository.Update(existingLocation);
            }
            else
            {
                await locationRepository.AddAsync(new DriverLocation
                {
                    DriverId = request.DriverId,
                    ActiveOrderId = request.ActiveOrderId,
                    Lat = request.Lat,
                    Lng = request.Lng,
                    RecordedAt = request.RecordedAt
                });
            }
            await unitOfWork.SaveChangesAsync(cancellationToken);
            return true;
        }
    }

}
