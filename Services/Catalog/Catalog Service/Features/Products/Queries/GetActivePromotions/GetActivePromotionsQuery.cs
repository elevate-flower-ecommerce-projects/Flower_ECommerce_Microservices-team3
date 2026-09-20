using Blocks.Contracts.Common;
using MediatR;

namespace Catalog_Service.Features.Products.Queries.GetActivePromotions
{
    public sealed record GetActivePromotionsQuery(
        IReadOnlyCollection<Guid> ProductIds,
        Guid? StoreId) 
        : IRequest<Result<IReadOnlyCollection<ProductPromotionResponse>>>;
}
