namespace Cart_Service.Services
{
    public interface ICatalogServiceClient
    {
        Task<CatalogProductInfo?> GetProductByIdAsync(Guid productId, CancellationToken ct = default);
        Task<Dictionary<Guid, CatalogProductInfo>> GetProductsByIdsAsync(IEnumerable<Guid> productIds, CancellationToken ct = default);
    }

    public sealed record CatalogProductInfo(
        Guid Id,
        string Name,
        string ImageUrl,
        decimal Price,
        bool InStock);
}
