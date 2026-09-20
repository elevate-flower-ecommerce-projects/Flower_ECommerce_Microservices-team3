using Blocks.Contracts.Common;
using Blocks.Contracts.Interfaces;
using Blocks.Domain.Errors;
using MediatR;
using Order___Fulfillment_Service.Entities;
using Order___Fulfillment_Service.Features.Orders.GetOrderTracking.DTOs;

namespace Order___Fulfillment_Service.Features.Orders.GetOrderTracking.Queries
{
    public sealed class GetOrderForTrackingQueryHandler(IGenericRepository<Order> orderRepository)
    : IRequestHandler<GetOrderForTrackingQuery, Result<OrderTrackingProjection>>
    {
        public async Task<Result<OrderTrackingProjection>> Handle(
            GetOrderForTrackingQuery request,
            CancellationToken cancellationToken)
        {
            var order = await orderRepository.FirstOrDefaultAsync(
                o => o.Id == request.OrderId
                     && o.CustomerId == request.CustomerId
                     && o.DeletedAt == null,
                o => new OrderTrackingProjection(
                    o.Id,
                    o.Status,
                    o.AssignedDriverId,
                    o.DriverName,
                    o.DriverPhone,
                    o.DriverPhotoUrl,
                    o.DeliveryLatitude,
                    o.DeliveryLongitude,
                    o.AddressLine,
                    o.EstimatedDeliveryAt),
                cancellationToken);

            if (order == null)
            {
                return Result.Failure<OrderTrackingProjection>(Error.NotFound("Order not found."));
            }
            return Result.Success(order);
        }
    }
}
