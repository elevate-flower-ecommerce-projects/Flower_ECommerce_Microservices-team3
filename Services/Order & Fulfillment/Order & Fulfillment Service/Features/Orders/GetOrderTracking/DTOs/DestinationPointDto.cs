namespace Order___Fulfillment_Service.Features.Orders.GetOrderTracking.DTOs
{
    public sealed record DestinationPointDto(
    double Lat,
    double Lng,
    string AddressLine
);
}
