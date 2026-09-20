using Blocks.Contracts.Common;
using Blocks.Contracts.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Order___Fulfillment_Service.Entities;
using Order___Fulfillment_Service.Entities.Enums;
using Order___Fulfillment_Service.Features.DriverFulfillment.AcceptOrder.Commands;
using Order___Fulfillment_Service.Features.DriverFulfillment.Common;
using Order___Fulfillment_Service.Persistence;

namespace Order___Fulfillment_Service.Features.DriverFulfillment.AcceptOrder.Commands;

public sealed class AcceptOrderCommandHandler(
    IGenericRepository<Order> orderRepository,
    FlowersOrderDbContext dbContext,
    IUnitOfWork unitOfWork)
    : IRequestHandler<AcceptOrderCommand, Result>
{
    public async Task<Result> Handle(AcceptOrderCommand request, CancellationToken ct)
    {
        return await unitOfWork.ExecuteAsync(async () =>
        {
            // 1. Check if driver already has an active delivery (PROJECTION)
            var hasActiveDelivery = await orderRepository.GetQueryable()
                .AsNoTracking()
                .AnyAsync(o => o.AssignedDriverId == request.DriverId
                    && (o.Status == OrderStatus.PickedUp
                     || o.Status == OrderStatus.OutForDelivery
                     || o.Status == OrderStatus.AwaitingDeliveryConfirmation), ct);

            if (hasActiveDelivery)
            {
                return Result.Failure(DriverErrors.AlreadyHasActiveDelivery());
            }

            // 2. Atomic claim via ExecuteUpdateAsync
            var now = DateTime.UtcNow;
            var rowsAffected = await dbContext.Orders
                .Where(o => o.Id == request.OrderId
                         && o.Status == OrderStatus.Preparing
                         && o.AssignedDriverId == null)
                .ExecuteUpdateAsync(setters => setters
                    .SetProperty(o => o.AssignedDriverId, request.DriverId)
                    .SetProperty(o => o.AssignedAt, now)
                    .SetProperty(o => o.UpdatedAt, now), ct);

            if (rowsAffected == 0)
            {
                // Determine whether order doesn't exist or was claimed / wrong status
                var exists = await orderRepository.GetQueryable()
                    .AsNoTracking()
                    .AnyAsync(o => o.Id == request.OrderId, ct);

                return exists
                    ? Result.Failure(DriverErrors.OrderAlreadyClaimed())
                    : Result.Failure(DriverErrors.OrderNotFound());
            }

            return Result.Success();
        }, ct);
    }
}
