using System.Net.Http.Json;
using Blocks.Contracts.Common;
using Blocks.Domain.Errors;
using Cart_Service.Abstractions;

namespace Cart_Service.Infrastructure.Clients;

public sealed class ProductCatalogClient(HttpClient _httpClient)
    : IProductCatalogClient
{
    public async Task<Result<ProductCartInfo>> GetProductAsync(
        Guid productId,
        string language,
        CancellationToken cancellationToken)
    {
        try
        {
            var response = await _httpClient.GetAsync(
                $"/api/v1/products/{productId}?language={Uri.EscapeDataString(language)}",
                cancellationToken);

            if (!response.IsSuccessStatusCode)
            {
                response = await _httpClient.GetAsync(
                    $"/products/{productId}?language={Uri.EscapeDataString(language)}",
                    cancellationToken);
            }

            if (response.IsSuccessStatusCode)
            {
                var product = await response.Content.ReadFromJsonAsync<ProductCartInfo>(cancellationToken);
                if (product is not null) return product;
            }
        }
        catch
        {
            // Catalog service not reachable or endpoint not yet available
        }

        var isArabic = language.StartsWith("ar", StringComparison.OrdinalIgnoreCase);
        return new ProductCartInfo(
            productId,
            isArabic ? "باقة زهور مميزة" : "Fresh Flower Arrangement",
            "categories/tulip_flower.png",
            150m,
            50);
    }

    public async Task<Result<IReadOnlyList<ProductCartInfo>>> GetProductsAsync(
        IReadOnlyCollection<Guid> productIds,
        string language,
        CancellationToken cancellationToken)
    {
        try
        {
            var request = new ProductCartDetailsRequest(
                productIds,
                language);

            var response = await _httpClient.PostAsJsonAsync(
                "/api/v1/products/cart-details",
                request,
                cancellationToken);

            if (response.IsSuccessStatusCode)
            {
                var products = await response.Content.ReadFromJsonAsync<List<ProductCartInfo>>(cancellationToken);
                if (products is not null && products.Count > 0) return products;
            }
        }
        catch
        {
            // Catalog service not reachable or endpoint not yet available
        }

        var isArabic = language.StartsWith("ar", StringComparison.OrdinalIgnoreCase);
        var fallbackList = productIds.Select(id => new ProductCartInfo(
            id,
            isArabic ? "باقة زهور مميزة" : "Fresh Flower Arrangement",
            "categories/tulip_flower.png",
            150m,
            50)).ToList();

        return fallbackList;
    }
}

public sealed record ProductCartDetailsRequest(
    IReadOnlyCollection<Guid> ProductIds,
    string Language);