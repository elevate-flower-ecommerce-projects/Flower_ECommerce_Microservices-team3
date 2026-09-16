using Blocks.Contracts.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Order___Fulfillment_Service.Entities;
using Order___Fulfillment_Service.Persistence;

namespace Order___Fulfillment_Service.Features.Orders.ConfirmDelivery.Commands
{
    public class ClearDriverActiveOrderCommandHandler(
    IGenericRepository<DriverLocation> locationRepository,
    IUnitOfWork unitOfWork)
    : IRequestHandler<ClearDriverActiveOrderCommand, bool>
    {
        public async Task<bool> Handle(
            ClearDriverActiveOrderCommand request,
            CancellationToken cancellationToken)
        {
            var driverLocation = await locationRepository.GetQueryable()
                .FirstOrDefaultAsync(l => l.DriverId == request.DriverId, cancellationToken);
            if (driverLocation != null)
            {
                driverLocation.ActiveOrderId = null; 
                locationRepository.Update(driverLocation);
                await unitOfWork.SaveChangesAsync(cancellationToken);
            }
            return true;
        }
    }
}
