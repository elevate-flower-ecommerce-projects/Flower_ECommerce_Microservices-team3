namespace Order___Fulfillment_Service.Services;

public record UserAddressDto(
    Guid Id,
    Guid CustomerId,
    string RecipientName,
    string RecipientPhone,
    string AddressLine,
    string City,
    string Area,
    double Latitude,
    double Longitude,
    bool IsDefault
);

public record StoreCoverageDto(
    Guid StoreId,
    string StoreName,
    bool IsServiceable,
    double DistanceKm,
    decimal DeliveryFee,
    int EstimatedDeliveryMinutes
);

public interface IAddressServiceClient
{
    Task<UserAddressDto?> GetAddressByIdAsync(Guid addressId, string? bearerToken = null, CancellationToken ct = default);
    Task<UserAddressDto?> GetDefaultAddressAsync(string? bearerToken = null, CancellationToken ct = default);
    Task<IReadOnlyList<UserAddressDto>> GetUserAddressesAsync(string? bearerToken = null, CancellationToken ct = default);
    Task<StoreCoverageDto?> GetNearestCoveringStoreAsync(double latitude, double longitude, CancellationToken ct = default);
}
