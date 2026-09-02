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

            var envelope = await response.Content.ReadFromJsonAsync<AddressApiResponseEnvelope<UserAddressDto>>(JsonOptions, ct);
            return envelope?.Success == true ? envelope.Data : null;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to fetch address {AddressId} from Address service", addressId);
            return null;
        }
    }

    public async Task<UserAddressDto?> GetDefaultAddressAsync(string? bearerToken = null, CancellationToken ct = default)
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
                return null;
            }

            var envelope = await response.Content.ReadFromJsonAsync<AddressApiResponseEnvelope<List<UserAddressDto>>>(JsonOptions, ct);
            return envelope?.Data?.FirstOrDefault(a => a.IsDefault) ?? envelope?.Data?.FirstOrDefault();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to fetch default address from Address service");
            return null;
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
            return new StoreCoverageDto(
                data.StoreId,
                data.StoreName,
                data.IsServiceable,
                data.DeliveryFee,
                data.EstimatedDeliveryMinutes);
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

    private sealed class NearestStoreApiData
    {
        public Guid StoreId { get; init; }
        public string StoreName { get; init; } = string.Empty;
        public bool IsServiceable { get; init; }
        public decimal DeliveryFee { get; init; }
        public int EstimatedDeliveryMinutes { get; init; } = 45;
    }
}
