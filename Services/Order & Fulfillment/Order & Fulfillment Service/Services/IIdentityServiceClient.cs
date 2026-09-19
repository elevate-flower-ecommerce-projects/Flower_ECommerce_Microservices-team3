namespace Order___Fulfillment_Service.Services
{

    public record DriverProfileDto(
    Guid DriverId,
    string FullName,
    string Phone,
    string? PhotoUrl
);
    public interface IIdentityServiceClient
    {
        Task<DriverProfileDto?> GetDriverProfileAsync(Guid driverId, CancellationToken ct = default);
    }
}
