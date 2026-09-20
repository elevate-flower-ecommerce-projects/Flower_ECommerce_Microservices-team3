using Blocks.Contracts.Common;

namespace Cart_Service.Abstractions;

public interface IProductCatalogClient
{
    Task<Result<ProductCartInfo>> GetProductAsync(
        Guid productId,
        string language,
        CancellationToken cancellationToken);

    Task<Result<IReadOnlyList<ProductCartInfo>>> GetProductsAsync(
        IReadOnlyCollection<Guid> productIds,
        string language,
        CancellationToken cancellationToken);
}

public sealed record ProductCartInfo(
    Guid ProductId,
    string ProductName,
    string ProductImageUrl,
    decimal UnitPrice,
    int AvailableStock);