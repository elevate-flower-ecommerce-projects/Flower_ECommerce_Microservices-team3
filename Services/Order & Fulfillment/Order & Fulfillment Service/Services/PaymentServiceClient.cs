using System.Net.Http.Headers;
using System.Text.Json;
using Order___Fulfillment_Service.Entities.Enums;

namespace Order___Fulfillment_Service.Services;

public sealed class PaymentServiceClient : IPaymentServiceClient
{
    private readonly HttpClient _httpClient;
    private readonly ILogger<PaymentServiceClient> _logger;
    private static readonly JsonSerializerOptions JsonOptions = new() { PropertyNameCaseInsensitive = true };

    public PaymentServiceClient(HttpClient httpClient, ILogger<PaymentServiceClient> logger)
    {
        _httpClient = httpClient;
        _logger = logger;
    }

    public async Task<CardSessionResultDto?> CreateCardSessionAsync(
        CreatePaymentSessionRequest request,
        string? bearerToken = null,
        CancellationToken ct = default)
    {
        try
        {
            using var httpRequest = new HttpRequestMessage(HttpMethod.Post, "/payments/sessions")
            {
                Content = JsonContent.Create(request)
            };

            if (!string.IsNullOrWhiteSpace(bearerToken))
            {
                httpRequest.Headers.Authorization = new AuthenticationHeaderValue("Bearer", bearerToken);
            }

            using var response = await _httpClient.SendAsync(httpRequest, ct);
            if (!response.IsSuccessStatusCode)
            {
                _logger.LogWarning("Payment service returned status code {StatusCode} for order {OrderId}", (int)response.StatusCode, request.OrderId);
                return null;
            }

            var envelope = await response.Content.ReadFromJsonAsync<PaymentApiResponseEnvelope<CardSessionResultDto>>(JsonOptions, ct);
            return envelope?.Success == true ? envelope.Data : null;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to call Payment service to create session for order {OrderId}", request.OrderId);
            return null;
        }
    }

    private sealed class PaymentApiResponseEnvelope<T>
    {
        public bool Success { get; init; }
        public T? Data { get; init; }
    }
}
