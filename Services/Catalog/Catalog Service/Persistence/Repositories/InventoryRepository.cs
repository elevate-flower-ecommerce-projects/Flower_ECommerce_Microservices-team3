using Catalog_Service.Features.Products.Queries.GetProductStock;
using Catalog_Service.Persistence.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Catalog_Service.Persistence.Repositories
{
    public sealed class InventoryRepository(FlowersCatalogDbContext context)
        : IInventoryRepository
    {
        public async Task<IReadOnlyCollection<ProductStockResponse>> GetProductsStockAsync(
            IReadOnlyCollection<Guid> productIds,
            CancellationToken cancellationToken)
        {
            return await context.Inventories
                .AsNoTracking()
                .Where(i => productIds.Contains(i.ProductId))
                .Select(i => new ProductStockResponse(
                    i.ProductId,
                    i.Quantity > 0))
                .ToListAsync(cancellationToken);
        }
    }
}
