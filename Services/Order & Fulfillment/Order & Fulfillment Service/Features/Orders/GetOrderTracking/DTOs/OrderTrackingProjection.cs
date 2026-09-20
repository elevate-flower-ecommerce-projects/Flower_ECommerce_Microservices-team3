using Order___Fulfillment_Service.Entities.Enums;

namespace Order___Fulfillment_Service.Features.Orders.GetOrderTracking.DTOs
{
    public sealed record OrderTrackingProjection(
    Guid Id,
    OrderStatus Status,
    Guid? AssignedDriverId,
    string? DriverName,
    string? DriverPhone,
    string? DriverPhotoUrl,
    double DeliveryLatitude,
    double DeliveryLongitude,
    string AddressLine,
    DateTime EstimatedDeliveryAt);
}
