using Blocks.Contracts.Pagination;
using Catalog_Service.Entities;
using Catalog_Service.Features.Products.Queries.GetProducts;
using Catalog_Service.Persistence.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Catalog_Service.Persistence.Repositories;
                                    
public sealed class ProductRepository : GenericRepository<Product>, IProductRepository
{
    public ProductRepository(FlowersCatalogDbContext context)
        : base(context)
    {
    }

    public async Task<PagedResult<ProductSummaryResponse>> GetProductsPageAsync(
        int page,
        int pageSize,
        Guid? categoryId,
        Guid? occasionId,
        Guid? storeId,
        CancellationToken cancellationToken)
    {
        var query = _context.Products
            .AsNoTracking()
            .Where(p => p.IsActive);

        if (categoryId.HasValue)
        {
            query = query.Where(p =>
                p.CategoryId == categoryId.Value);
        }

        if (occasionId.HasValue)
        {
            query = query.Where(p =>
                p.ProductOccasions.Any(po =>
                    po.OccasionId == occasionId.Value));
        }

        var totalCount = await query
            .CountAsync(cancellationToken);

        var items = await query
            .OrderBy(p => p.Name)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .Select(p => new ProductSummaryResponse(
                p.Id,
                p.Name,
                p.ImageUrl,
                p.Price,
                null,
                null,
                false))
            .ToListAsync(cancellationToken);

        var pagination = new PaginationParams
        {
            PageNumber = page,
            PageSize = pageSize
        };

        return PagedResult<ProductSummaryResponse>.Create(
            items,
            totalCount,
            pagination);
    }
}