using System.Text.Json;

namespace Cart_Service.Services
{
    public sealed class CatalogServiceClient : ICatalogServiceClient
    {
        private readonly HttpClient _httpClient;
        private readonly ILogger<CatalogServiceClient> _logger;

        private static readonly JsonSerializerOptions JsonOptions = new()
        {
            PropertyNameCaseInsensitive = true
        };

        public CatalogServiceClient(HttpClient httpClient, ILogger<CatalogServiceClient> logger)
        {
            _httpClient = httpClient;
            _logger = logger;
        }

        public async Task<CatalogProductInfo?> GetProductByIdAsync(Guid productId, CancellationToken ct = default)
        {
            try
            {
                using var response = await _httpClient.GetAsync($"/products/{productId}", ct);

                if (!response.IsSuccessStatusCode)
                {
                    _logger.LogWarning(
                        "Catalog service returned {StatusCode} for product {ProductId}",
                        (int)response.StatusCode, productId);
                    return null;
                }

                var apiResponse = await response.Content
                    .ReadFromJsonAsync<CatalogApiEnvelope>(JsonOptions, ct);

                if (apiResponse?.Success != true || apiResponse.Data is null)
                    return null;

                var data = apiResponse.Data;

                return new CatalogProductInfo(
                    data.Id,
                    data.Name,
                    data.ImageUrl,
                    data.Price,
                    string.Equals(data.Status, "InStock", StringComparison.OrdinalIgnoreCase));
            }
            catch (Exception ex) when (ex is HttpRequestException or TaskCanceledException or JsonException)
            {
                _logger.LogWarning(
                    ex,
                    "Failed to fetch product {ProductId} from Catalog service",
                    productId);
                return null;
            }
        }

        public async Task<Dictionary<Guid, CatalogProductInfo>> GetProductsByIdsAsync(
            IEnumerable<Guid> productIds, CancellationToken ct = default)
        {
            var tasks = productIds
                .Distinct()
                .Select(id => GetProductByIdAsync(id, ct));

            var results = await Task.WhenAll(tasks);

            return results
                .Where(r => r is not null)
                .ToDictionary(r => r!.Id, r => r!);
        }

        private sealed class CatalogApiEnvelope
        {
            public bool Success { get; init; }
            public CatalogProductData? Data { get; init; }
        }

        private sealed class CatalogProductData
        {
            public Guid Id { get; init; }
            public string Name { get; init; } = string.Empty;
            public string ImageUrl { get; init; } = string.Empty;
            public decimal Price { get; init; }
            public string Status { get; init; } = string.Empty;
        }
    }
}
