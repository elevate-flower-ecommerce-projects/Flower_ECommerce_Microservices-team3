using Blocks.Contracts.Common;
using Catalog_Service.Persistence.Repositories.Interfaces;
using MediatR;

namespace Catalog_Service.Features.Products.Queries.GetActivePromotions
{
    public sealed class GetActivePromotionsHandler(IPromotionRepository _promotionRepository)
        : IRequestHandler<GetActivePromotionsQuery,
            Result<IReadOnlyCollection<ProductPromotionResponse>>>
    {

        public async Task<Result<IReadOnlyCollection<ProductPromotionResponse>>> Handle(
            GetActivePromotionsQuery request,
            CancellationToken cancellationToken)
        {
            var promotions = await _promotionRepository
                .GetActivePromotionsAsync(
                    request.ProductIds,
                    request.StoreId,
                    cancellationToken);

            return Result.Success(promotions);
        }
    }
}
