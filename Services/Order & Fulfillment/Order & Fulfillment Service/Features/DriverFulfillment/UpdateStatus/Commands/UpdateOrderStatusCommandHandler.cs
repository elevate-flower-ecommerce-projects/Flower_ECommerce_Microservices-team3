using Blocks.Contracts.Common;
using Blocks.Contracts.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Order___Fulfillment_Service.Entities;
using Order___Fulfillment_Service.Entities.Enums;
using Order___Fulfillment_Service.Features.DriverFulfillment.Common;
using Order___Fulfillment_Service.Features.DriverFulfillment.UpdateStatus.Commands;
using Order___Fulfillment_Service.Persistence;

namespace Order___Fulfillment_Service.Features.DriverFulfillment.UpdateStatus.Commands;

public sealed class UpdateOrderStatusCommandHandler(
    IGenericRepository<Order> orderRepository,
    FlowersOrderDbContext dbContext,
    IUnitOfWork unitOfWork)
    : IRequestHandler<UpdateOrderStatusCommand, Result>
{
    public async Task<Result> Handle(UpdateOrderStatusCommand request, CancellationToken ct)
    {
        return await unitOfWork.ExecuteAsync(async () =>
        {
            // 1. Read current status + assigned driver using projection
            var orderInfo = await orderRepository.GetQueryable()
                .AsNoTracking()
                .Where(o => o.Id == request.OrderId)
                .Select(o => new { o.Status, o.AssignedDriverId })
                .FirstOrDefaultAsync(ct);

            if (orderInfo is null)
            {
                return Result.Failure(DriverErrors.OrderNotFound());
            }

            // 2. Ownership check
            if (orderInfo.AssignedDriverId != request.DriverId)
            {
                return Result.Failure(DriverErrors.OrderNotAssignedToYou());
            }

            // 3. Map DriverStatusUpdate to OrderStatus
            var targetStatus = request.NewStatus switch
            {
                DriverStatusUpdate.PickedUp => OrderStatus.PickedUp,
                DriverStatusUpdate.OutForDelivery => OrderStatus.OutForDelivery,
                DriverStatusUpdate.AwaitingDeliveryConfirmation => OrderStatus.AwaitingDeliveryConfirmation,
                _ => (OrderStatus?)null
            };

            if (targetStatus is null)
            {
                return Result.Failure(DriverErrors.InvalidStatusTransition(
                    orderInfo.Status.ToString(), request.NewStatus.ToString()));
            }

            // 4. Validate allowed state transitions
            var isValidTransition = (orderInfo.Status, targetStatus.Value) switch
            {
                (OrderStatus.Preparing, OrderStatus.PickedUp) => true,
                (OrderStatus.PickedUp, OrderStatus.OutForDelivery) => true,
                (OrderStatus.OutForDelivery, OrderStatus.AwaitingDeliveryConfirmation) => true,
                _ => false
            };

            if (!isValidTransition)
            {
                return Result.Failure(DriverErrors.InvalidStatusTransition(
                    orderInfo.Status.ToString(), targetStatus.Value.ToString()));
            }

            // 5. Update status atomically via ExecuteUpdateAsync
            var now = DateTime.UtcNow;
            await dbContext.Orders
                .Where(o => o.Id == request.OrderId)
                .ExecuteUpdateAsync(setters => setters
                    .SetProperty(o => o.Status, targetStatus.Value)
                    .SetProperty(o => o.UpdatedAt, now), ct);

            return Result.Success();
        }, ct);
    }
}
