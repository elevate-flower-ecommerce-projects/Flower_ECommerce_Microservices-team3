using Blocks.Contracts.Pagination;
using Catalog_Service.Features.Products.Queries.GetProducts;

namespace Catalog_Service.Persistence.Repositories.Interfaces;

public interface IProductRepository
{
    Task<PagedResult<ProductSummaryResponse>> GetProductsPageAsync(
        int page,
        int pageSize,
        Guid? categoryId,
        Guid? occasionId,
        Guid? storeId,
        CancellationToken cancellationToken);
}