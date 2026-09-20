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
        var response = await _httpClient.GetAsync(
            $"/api/v1/products/{productId}?language={Uri.EscapeDataString(language)}",
            cancellationToken);

        if (!response.IsSuccessStatusCode)
        {
            return Error.NotFound(
                "Product was not found.");
        }

        var product =
            await response.Content.ReadFromJsonAsync<ProductCartInfo>(
                cancellationToken);

        if (product is null)
        {
            return Error.Internal(
                "Invalid response from Catalog Service.");
        }

        return product;
    }

    public async Task<Result<IReadOnlyList<ProductCartInfo>>> GetProductsAsync(
        IReadOnlyCollection<Guid> productIds,
        string language,
        CancellationToken cancellationToken)
    {
        var request = new ProductCartDetailsRequest(
            productIds,
            language);

        var response = await _httpClient.PostAsJsonAsync(
            "/api/v1/products/cart-details",
            request,
            cancellationToken);

        if (!response.IsSuccessStatusCode)
        {
            return Error.Internal(
                "Failed to retrieve products from Catalog Service.");
        }

        var products =
            await response.Content
                .ReadFromJsonAsync<List<ProductCartInfo>>(
                    cancellationToken);

        if (products is null)
        {
            return Error.Internal(
                "Invalid response from Catalog Service.");
        }

        return products;
    }
}

public sealed record ProductCartDetailsRequest(
    IReadOnlyCollection<Guid> ProductIds,
    string Language);