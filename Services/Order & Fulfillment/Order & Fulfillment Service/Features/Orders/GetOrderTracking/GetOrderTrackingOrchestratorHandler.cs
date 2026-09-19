using Blocks.Contracts.Common;
using Blocks.Domain.Errors;
using MediatR;
using Order___Fulfillment_Service.Entities.Enums;
using Order___Fulfillment_Service.Features.Orders.GetOrderTracking.Commands;
using Order___Fulfillment_Service.Features.Orders.GetOrderTracking.DTOs;
using Order___Fulfillment_Service.Features.Orders.GetOrderTracking.Queries;
using Order___Fulfillment_Service.Services;

namespace Order___Fulfillment_Service.Features.Orders.GetOrderTracking
{
    public sealed class GetOrderTrackingOrchestratorHandler(
    ISender sender,
    IIdentityServiceClient identityServiceClient)
    : IRequestHandler<GetOrderTrackingOrchestrator, Result<OrderTrackingDataDto>>
    {
        private static readonly HashSet<OrderStatus> LiveStatuses = new()
    {
        OrderStatus.PickedUp,
        OrderStatus.OutForDelivery,
        OrderStatus.AwaitingDeliveryConfirmation
    };
        public async Task<Result<OrderTrackingDataDto>> Handle(
            GetOrderTrackingOrchestrator request,
            CancellationToken cancellationToken)
        {
           
            var orderResult = await sender.Send(
                new GetOrderForTrackingQuery(request.OrderId, request.CustomerId),
                cancellationToken);
            if (orderResult.IsFailure)
            {
                return Result.Failure<OrderTrackingDataDto>(orderResult.Error!);
            }
            var order = orderResult.Value;

            if (order.Status == OrderStatus.Delivered)
            {
                return Result.Failure<OrderTrackingDataDto>(
                    Error.Validation("This order has already been delivered."));
            }

            if (order.Status == OrderStatus.Cancelled)
            {
                return Result.Failure<OrderTrackingDataDto>(
                    Error.Validation("This order has been cancelled."));
            }

            if (order.AssignedDriverId == null)
            {
                return Result.Failure<OrderTrackingDataDto>(
                    Error.Validation("Tracking is only available once a driver accepts the order."));
            }
          
            if (order.AssignedDriverId != null && order.DriverName == null)
            {
                order = await RefreshDriverSnapshotAsync(order, order.AssignedDriverId.Value, cancellationToken);
            }
            bool isLive = LiveStatuses.Contains(order.Status);
         
            var driver = new DriverSummaryDto(
                order.AssignedDriverId.Value,
                order.DriverName ?? "Driver",
                order.DriverPhone ?? string.Empty,
                order.DriverPhotoUrl);
          
            LocationPointDto? currentLocation = null;
            if (isLive && order.AssignedDriverId != null)
            {
                var locationResult = await sender.Send(
                    new GetLatestDriverLocationQuery(order.Id, order.AssignedDriverId.Value),
                    cancellationToken);
                if (locationResult.IsFailure)
                {
                    return Result.Failure<OrderTrackingDataDto>(locationResult.Error!);
                }
                currentLocation = locationResult.Value;
            }
          
            var response = new OrderTrackingDataDto(
                OrderId: order.Id,
                Status: order.Status,
                IsLive: isLive,
                Driver: driver,
                CurrentLocation: currentLocation,
                UserAddress: new DestinationPointDto(
                    order.DeliveryLatitude,
                    order.DeliveryLongitude,
                    order.AddressLine),
                EstimatedDeliveryAt: order.EstimatedDeliveryAt,
                AwaitingCustomerConfirmation: order.Status == OrderStatus.AwaitingDeliveryConfirmation
            );
            return Result.Success(response);
        }
        private async Task<OrderTrackingProjection> RefreshDriverSnapshotAsync(
            OrderTrackingProjection order,
            Guid driverId,
            CancellationToken cancellationToken)
        {
            var profile = await identityServiceClient.GetDriverProfileAsync(driverId, cancellationToken);
            if (profile == null)
            {
                return order;
            }
            var snapshotResult = await sender.Send(
                new SaveOrderDriverSnapshotCommand(order.Id, profile.FullName, profile.Phone, profile.PhotoUrl),
                cancellationToken);
            if (snapshotResult.IsFailure || snapshotResult.Value == null)
            {
                return order;
            }
            return order with
            {
                DriverName = snapshotResult.Value.Name,
                DriverPhone = snapshotResult.Value.Phone,
                DriverPhotoUrl = snapshotResult.Value.PhotoUrl
            };
        }
    }

}
