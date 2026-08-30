using Blocks.Contracts.Pagination;
using Catalog_Service.Entities;
using Catalog_Service.Entities.Enums;
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
        ProductSort? sort,
        CancellationToken cancellationToken)
    {
        var query = _context.Products
            .AsNoTracking()
            .Where(p => p.IsActive);

        // Category filter
        if (categoryId.HasValue)
        {
            query = query.Where(p =>
                p.CategoryId == categoryId.Value);
        }

        // Occasion filter
        if (occasionId.HasValue)
        {
            query = query.Where(p =>
                p.ProductOccasions.Any(po =>
                    po.OccasionId == occasionId.Value));
        }

        // Total count before pagination
        var totalCount = await query
            .CountAsync(cancellationToken);

        // Sorting
        query = sort switch
        {
            ProductSort.PriceLowToHigh =>
                query
                    .OrderBy(p => p.Price)
                    .ThenBy(p => p.Id),

            ProductSort.PriceHighToLow =>
                query
                    .OrderByDescending(p => p.Price)
                    .ThenBy(p => p.Id),

            ProductSort.NewestFirst =>
                query
                    .OrderByDescending(p => p.CreatedAt)
                    .ThenBy(p => p.Id),

            ProductSort.OldestFirst =>
                query
                    .OrderBy(p => p.CreatedAt)
                    .ThenBy(p => p.Id),

            // Preserve existing default behavior
            _ =>
                query
                    .OrderBy(p => p.Name)
                    .ThenBy(p => p.Id)
        };

        // Pagination + Projection
        var items = await query
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