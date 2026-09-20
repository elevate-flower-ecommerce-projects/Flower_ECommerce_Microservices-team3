using Blocks.Contracts.Common;
using Catalog_Service.Persistence.Repositories.Interfaces;
using MediatR;

namespace Catalog_Service.Features.Products.Queries.GetProductStock
{
    public sealed class GetProductStockHandler(IInventoryRepository _inventoryRepository)
        : IRequestHandler<
            GetProductStockQuery,
            Result<IReadOnlyCollection<ProductStockResponse>>>
    {
        public async Task<Result<IReadOnlyCollection<ProductStockResponse>>> Handle(
            GetProductStockQuery request,
            CancellationToken cancellationToken)
        {
            var stock = await _inventoryRepository.GetProductsStockAsync(
                request.ProductIds,
                cancellationToken);

            return Result.Success(stock);
        }
    }
}
