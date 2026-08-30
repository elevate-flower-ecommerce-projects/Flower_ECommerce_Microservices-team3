using Catalog_Service.Features.Products.Queries.GetActivePromotions;

namespace Catalog_Service.Persistence.Repositories.Interfaces
{
    public interface IPromotionRepository
    {
        Task<IReadOnlyCollection<ProductPromotionResponse>> GetActivePromotionsAsync(
            IReadOnlyCollection<Guid> productIds,
            Guid? storeId,
            CancellationToken cancellationToken);
    }
}
