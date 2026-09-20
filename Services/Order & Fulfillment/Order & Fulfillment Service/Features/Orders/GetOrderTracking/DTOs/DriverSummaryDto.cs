namespace Order___Fulfillment_Service.Features.Orders.GetOrderTracking.DTOs
{
    public sealed record DriverSummaryDto(
     Guid DriverId,
     string Name,
     string Phone,
     string? PhotoUrl
 );

}
