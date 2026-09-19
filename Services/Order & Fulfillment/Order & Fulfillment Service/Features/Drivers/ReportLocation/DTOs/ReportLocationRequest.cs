namespace Order___Fulfillment_Service.Features.Drivers.ReportLocation.DTOs
{
    public sealed record ReportLocationRequest(double Lat, double Lng, DateTime RecordedAt);
}
