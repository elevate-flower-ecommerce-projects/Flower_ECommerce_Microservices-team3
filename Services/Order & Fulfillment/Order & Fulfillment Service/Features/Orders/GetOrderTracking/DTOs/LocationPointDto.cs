namespace Order___Fulfillment_Service.Features.Orders.GetOrderTracking.DTOs
{
    public sealed record LocationPointDto(
    double Lat,
    double Lng,
    DateTime RecordedAt,
    bool IsStale
);

}
