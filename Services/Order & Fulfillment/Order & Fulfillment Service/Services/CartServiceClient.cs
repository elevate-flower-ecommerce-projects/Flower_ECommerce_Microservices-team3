using System.Net.Http.Headers;
using System.Text.Json;

namespace Order___Fulfillment_Service.Services;

public sealed class CartServiceClient : ICartServiceClient
{
    private readonly HttpClient _httpClient;
    private readonly ILogger<CartServiceClient> _logger;
    private static readonly JsonSerializerOptions JsonOptions = new() { PropertyNameCaseInsensitive = true };

    public CartServiceClient(HttpClient httpClient, ILogger<CartServiceClient> logger)
    {
        _httpClient = httpClient;
        _logger = logger;
    }

    public async Task<CartDto?> GetCartByIdAsync(Guid cartId, string? bearerToken = null, CancellationToken ct = default)
    {
        try
        {
            using var request = new HttpRequestMessage(HttpMethod.Get, $"/api/cart?cartId={cartId}");
            if (!string.IsNullOrWhiteSpace(bearerToken))
            {
                request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", bearerToken);
            }

            using var response = await _httpClient.SendAsync(request, ct);
            if (!response.IsSuccessStatusCode)
            {
                _logger.LogWarning("Cart service returned status code {StatusCode} for cart {CartId}", (int)response.StatusCode, cartId);
                return null;
            }

            var envelope = await response.Content.ReadFromJsonAsync<CartApiResponseEnvelope>(JsonOptions, ct);
            if (envelope?.Success != true || envelope.Data is null)
            {
                return null;
            }

            var data = envelope.Data;
            var items = data.Items.Select(i => new CartItemDto(i.ProductId, i.ProductName, i.Quantity, i.UnitPrice)).ToList();
            return new CartDto(data.Id, data.CustomerId, data.Subtotal, items);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to call Cart service for cart {CartId}", cartId);
            return null;
        }
    }

    public async Task<bool> ClearCartAsync(Guid cartId, string? bearerToken = null, CancellationToken ct = default)
    {
        try
        {
            using var request = new HttpRequestMessage(HttpMethod.Delete, $"/api/cart?cartId={cartId}");
            if (!string.IsNullOrWhiteSpace(bearerToken))
            {
                request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", bearerToken);
            }

            using var response = await _httpClient.SendAsync(request, ct);
            return response.IsSuccessStatusCode;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to clear cart {CartId} in Cart service", cartId);
            return false;
        }
    }

    private sealed class CartApiResponseEnvelope
    {
        public bool Success { get; init; }
        public CartApiData? Data { get; init; }
    }

    private sealed class CartApiData
    {
        public Guid Id { get; init; }
        public Guid CustomerId { get; init; }
        public decimal Subtotal { get; init; }
        public List<CartApiItemData> Items { get; init; } = [];
    }

    private sealed class CartApiItemData
    {
        public Guid ProductId { get; init; }
        public string ProductName { get; init; } = string.Empty;
        public int Quantity { get; init; }
        public decimal UnitPrice { get; init; }
    }
}
