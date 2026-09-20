using System.Text.Json;

namespace Order___Fulfillment_Service.Services;

public sealed class IdentityServiceClient : IIdentityServiceClient
{
    private const string DriverProfilePathFormat = "/api/drivers/{0}/profile";

    private readonly HttpClient _httpClient;
    private readonly ILogger<IdentityServiceClient> _logger;
    private static readonly JsonSerializerOptions JsonOptions = new() { PropertyNameCaseInsensitive = true };

    public IdentityServiceClient(HttpClient httpClient, ILogger<IdentityServiceClient> logger)
    {
        _httpClient = httpClient;
        _logger = logger;
    }

    public async Task<DriverProfileDto?> GetDriverProfileAsync(Guid driverId, CancellationToken ct = default)
    {
        try
        {
            var path = string.Format(DriverProfilePathFormat, driverId);

            using var request = new HttpRequestMessage(HttpMethod.Get, path);
            using var response = await _httpClient.SendAsync(request, ct);

            if (!response.IsSuccessStatusCode)
            {
                _logger.LogWarning(
                    "Identity service returned status code {StatusCode} for driver profile {DriverId}",
                    (int)response.StatusCode, driverId);
                return null;
            }

            var envelope = await response.Content
                .ReadFromJsonAsync<IdentityApiResponseEnvelope<RawDriverProfileDto>>(JsonOptions, ct);

            if (envelope?.Success != true || envelope.Data is null)
            {
                return null;
            }

            return envelope.Data.ToDriverProfileDto();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to fetch driver profile {DriverId} from Identity service", driverId);
            return null;
        }
    }

    private sealed class IdentityApiResponseEnvelope<T>
    {
        public bool Success { get; init; }
        public T? Data { get; init; }
    }

    private sealed class RawDriverProfileDto
    {
        public Guid Id { get; init; }
        public string FirstName { get; init; } = string.Empty;
        public string LastName { get; init; } = string.Empty;
        public string? Phone { get; init; }
        public string? PhotoUrl { get; init; }

        public DriverProfileDto ToDriverProfileDto()
        {
            var fullName = $"{FirstName} {LastName}".Trim();

            return new DriverProfileDto(
                Id,
                string.IsNullOrWhiteSpace(fullName) ? "Driver" : fullName,
                Phone ?? string.Empty,
                PhotoUrl
            );
        }
    }
}
