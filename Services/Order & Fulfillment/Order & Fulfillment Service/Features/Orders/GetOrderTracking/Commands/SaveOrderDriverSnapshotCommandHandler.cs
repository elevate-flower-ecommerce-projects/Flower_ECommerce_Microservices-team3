using Blocks.Contracts.Common;
using Blocks.Contracts.Interfaces;
using MediatR;
using Order___Fulfillment_Service.Entities;
using Order___Fulfillment_Service.Features.Orders.GetOrderTracking.DTOs;
using Order___Fulfillment_Service.Persistence;

namespace Order___Fulfillment_Service.Features.Orders.GetOrderTracking.Commands
{
    public sealed class SaveOrderDriverSnapshotCommandHandler(
     IGenericRepository<Order> orderRepository,
     IUnitOfWork unitOfWork)
     : IRequestHandler<SaveOrderDriverSnapshotCommand, Result<DriverSnapshotDto?>>
    {
        public async Task<Result<DriverSnapshotDto?>> Handle(
            SaveOrderDriverSnapshotCommand request,
            CancellationToken cancellationToken)
        {
            var trackedOrder = await orderRepository.GetByIdAsync(request.OrderId, cancellationToken);
            if (trackedOrder == null)
            {
                return Result.Success<DriverSnapshotDto?>(null);
            }
           
            if (trackedOrder.DriverName != null)
            {
                return Result.Success<DriverSnapshotDto?>(
                    new DriverSnapshotDto(trackedOrder.DriverName, trackedOrder.DriverPhone, trackedOrder.DriverPhotoUrl));
            }
            trackedOrder.DriverName = request.DriverName;
            trackedOrder.DriverPhone = request.DriverPhone;
            trackedOrder.DriverPhotoUrl = request.DriverPhotoUrl;
            await unitOfWork.SaveChangesAsync(cancellationToken);
            return Result.Success<DriverSnapshotDto?>(
                new DriverSnapshotDto(request.DriverName, request.DriverPhone, request.DriverPhotoUrl));
        }
    }
}
