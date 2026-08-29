using Catalog_Service.Features.Products.Queries.GetActivePromotions;
using Catalog_Service.Persistence.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Catalog_Service.Persistence.Repositories
{
    public sealed class PromotionRepository(FlowersCatalogDbContext _context) : IPromotionRepository
    {

        public async Task<IReadOnlyCollection<ProductPromotionResponse>> GetActivePromotionsAsync(
            IReadOnlyCollection<Guid> productIds,
            Guid? storeId,
            CancellationToken cancellationToken)
        {
            var now = DateTime.UtcNow;

            return await _context.Promotions.AsNoTracking()
                .Where(p =>
                    productIds.Contains(p.ProductId) &&
                    p.IsActive &&
                    p.StartDate <= now &&
                    p.EndDate >= now &&
                    (!storeId.HasValue || p.StoreId == storeId))
                .Select(p => new ProductPromotionResponse(
                    p.ProductId,
                    p.DiscountPercent))
                .ToListAsync(cancellationToken);
        }
    }
}
