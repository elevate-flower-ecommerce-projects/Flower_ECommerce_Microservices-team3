using Order___Fulfillment_Service.Entities.Enums;

namespace Order___Fulfillment_Service.Features.Orders.GetOrderTracking.DTOs
{
    public sealed record OrderTrackingDataDto(
    Guid OrderId,
    OrderStatus Status,
    bool IsLive,
    DriverSummaryDto? Driver,
    LocationPointDto? CurrentLocation,
    DestinationPointDto UserAddress,
    DateTime? EstimatedDeliveryAt,
    bool AwaitingCustomerConfirmation
);
}
