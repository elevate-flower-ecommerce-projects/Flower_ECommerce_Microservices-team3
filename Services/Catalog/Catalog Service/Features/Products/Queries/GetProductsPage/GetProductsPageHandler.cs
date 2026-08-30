using Blocks.Contracts.Common;
using Blocks.Contracts.Pagination;
using Catalog_Service.Features.Products.Queries.GetProducts;
using Catalog_Service.Persistence.Repositories.Interfaces;
using MediatR;

namespace Catalog_Service.Features.Products.Queries.GetProductsPage
{
    public sealed class GetProductsPageHandler(
        IProductRepository _productRepository)
        : IRequestHandler<
            GetProductsPageQuery,
            Result<PagedResult<ProductSummaryResponse>>>
    {
        public async Task<Result<PagedResult<ProductSummaryResponse>>> Handle(
            GetProductsPageQuery request,
            CancellationToken cancellationToken)
        {
            var result = await _productRepository.GetProductsPageAsync(
                request.Page,
                request.PageSize,
                request.CategoryId,
                request.OccasionId,
                request.StoreId,
                request.Sort,
                cancellationToken);

            return Result.Success(result);
        }
    }
}
