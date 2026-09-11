using System.Net.Http.Headers;
using System.Text.Json;

namespace Order___Fulfillment_Service.Services;

public sealed class AddressServiceClient : IAddressServiceClient
{
    private readonly HttpClient _httpClient;
    private readonly ILogger<AddressServiceClient> _logger;
    private static readonly JsonSerializerOptions JsonOptions = new() { PropertyNameCaseInsensitive = true };

    public AddressServiceClient(HttpClient httpClient, ILogger<AddressServiceClient> logger)
    {
        _httpClient = httpClient;
        _logger = logger;
    }

    public async Task<UserAddressDto?> GetAddressByIdAsync(Guid addressId, string? bearerToken = null, CancellationToken ct = default)
    {
        try
        {
            using var request = new HttpRequestMessage(HttpMethod.Get, $"/users/me/addresses/{addressId}");
            if (!string.IsNullOrWhiteSpace(bearerToken))
            {
                request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", bearerToken);
            }

            using var response = await _httpClient.SendAsync(request, ct);
            if (!response.IsSuccessStatusCode)
            {
                _logger.LogWarning("Address service returned status code {StatusCode} for address {AddressId}", (int)response.StatusCode, addressId);
                return null;
            }

            var envelope = await response.Content.ReadFromJsonAsync<AddressApiResponseEnvelope<RawAddressDto>>(JsonOptions, ct);
            return envelope?.Success == true && envelope.Data is not null ? envelope.Data.ToUserAddressDto() : null;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to fetch address {AddressId} from Address service", addressId);
            return null;
        }
    }

    public async Task<UserAddressDto?> GetDefaultAddressAsync(string? bearerToken = null, CancellationToken ct = default)
    {
        var addresses = await GetUserAddressesAsync(bearerToken, ct);
        return addresses.FirstOrDefault(a => a.IsDefault) ?? addresses.FirstOrDefault();
    }

    public async Task<IReadOnlyList<UserAddressDto>> GetUserAddressesAsync(string? bearerToken = null, CancellationToken ct = default)
    {
        try
        {
            using var request = new HttpRequestMessage(HttpMethod.Get, "/users/me/addresses");
            if (!string.IsNullOrWhiteSpace(bearerToken))
            {
                request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", bearerToken);
            }

            using var response = await _httpClient.SendAsync(request, ct);
            if (!response.IsSuccessStatusCode)
            {
                _logger.LogWarning("Address service returned status code {StatusCode} for user addresses", (int)response.StatusCode);
                return [];
            }

            var envelope = await response.Content.ReadFromJsonAsync<AddressApiResponseEnvelope<List<RawAddressDto>>>(JsonOptions, ct);
            if (envelope?.Success != true || envelope.Data is null)
            {
                return [];
            }

            return envelope.Data.Select(a => a.ToUserAddressDto()).ToList();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to fetch user addresses from Address service");
            return [];
        }
    }

    public async Task<StoreCoverageDto?> GetNearestCoveringStoreAsync(double latitude, double longitude, CancellationToken ct = default)
    {
        try
        {
            using var response = await _httpClient.GetAsync($"/api/stores/nearest?latitude={latitude}&longitude={longitude}", ct);
            if (!response.IsSuccessStatusCode)
            {
                _logger.LogWarning("Address service returned status code {StatusCode} for nearest store", (int)response.StatusCode);
                return null;
            }

            var envelope = await response.Content.ReadFromJsonAsync<AddressApiResponseEnvelope<NearestStoreApiData>>(JsonOptions, ct);
            if (envelope?.Success != true || envelope.Data is null)
            {
                return null;
            }

            var data = envelope.Data;
            // Dynamic delivery time estimation:
            // Base preparation (15m) + Courier transit (3m per km, min 5m) + Dispatch/parking buffer (10m)
            var transitMinutes = Math.Max((int)Math.Ceiling(data.DistanceKm * 3.0), 5);
            var estimatedDeliveryMinutes = data.EstimatedDeliveryMinutes > 0
                ? data.EstimatedDeliveryMinutes
                : 15 + transitMinutes + 10;

            return new StoreCoverageDto(
                data.StoreId,
                !string.IsNullOrWhiteSpace(data.StoreName) ? data.StoreName : "Main Store",
                data.IsServiceable,
                data.DistanceKm,
                data.DeliveryFee > 0 ? data.DeliveryFee : 15.00m,
                estimatedDeliveryMinutes);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to resolve nearest covering store for lat: {Lat}, lng: {Lng}", latitude, longitude);
            return null;
        }
    }

    private sealed class AddressApiResponseEnvelope<T>
    {
        public bool Success { get; init; }
        public T? Data { get; init; }
    }

    private sealed class RawAddressDto
    {
        public Guid Id { get; init; }
        public Guid CustomerId { get; init; }
        public string RecipientName { get; init; } = string.Empty;
        public string? Phone { get; init; }
        public string? RecipientPhone { get; init; }
        public string AddressLine { get; init; } = string.Empty;
        public string City { get; init; } = string.Empty;
        public string Area { get; init; } = string.Empty;
        public double Lat { get; init; }
        public double Latitude { get; init; }
        public double Lng { get; init; }
        public double Longitude { get; init; }
        public bool IsDefault { get; init; }

        public UserAddressDto ToUserAddressDto() =>
            new(
                Id,
                CustomerId,
                RecipientName,
                !string.IsNullOrWhiteSpace(Phone) ? Phone : (RecipientPhone ?? string.Empty),
                AddressLine,
                City,
                Area,
                Latitude != 0 ? Latitude : Lat,
                Longitude != 0 ? Longitude : Lng,
                IsDefault
            );
    }

    private sealed class NearestStoreApiData
    {
        public Guid StoreId { get; init; }
        public string StoreName { get; init; } = string.Empty;
        public double DistanceKm { get; init; }
        public bool IsServiceable { get; init; } = true;
        public decimal DeliveryFee { get; init; } = 15.00m;
        public int EstimatedDeliveryMinutes { get; init; }
    }
}
