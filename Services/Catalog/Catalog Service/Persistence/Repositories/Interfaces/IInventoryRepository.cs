using Catalog_Service.Features.Products.Queries.GetProductStock;

namespace Catalog_Service.Persistence.Repositories.Interfaces
{
    public interface IInventoryRepository
    {
        Task<IReadOnlyCollection<ProductStockResponse>> GetProductsStockAsync(
            IReadOnlyCollection<Guid> productIds,
            CancellationToken cancellationToken);
    }
}
