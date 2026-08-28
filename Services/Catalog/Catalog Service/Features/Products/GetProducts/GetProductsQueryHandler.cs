using Blocks.Contracts.Common;
using Blocks.Contracts.Pagination;
using Catalog_Service.Entities;
using Catalog_Service.Features.Products.GetProductsByOccasionId.ViewModels;
using Catalog_Service.Persistence;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Catalog_Service.Features.Products.GetProducts;

public class GetProductsQueryHandler : IRequestHandler<GetProductsQuery, Result<PagedResult<ProductViewModel>>>
{
    private readonly FlowersCatalogDbContext _context;

    public GetProductsQueryHandler(FlowersCatalogDbContext context)
    {
        _context = context;
    }

    public async Task<Result<PagedResult<ProductViewModel>>> Handle(GetProductsQuery request, CancellationToken cancellationToken)
    {
        var isArabic = request.Language.StartsWith("ar", StringComparison.OrdinalIgnoreCase);

        var query = _context.Products.AsNoTracking().AsQueryable();

        // 1. Filter by CategoryId
        if (request.CategoryId.HasValue && request.CategoryId.Value != Guid.Empty)
        {
            query = query.Where(p => p.CategoryId == request.CategoryId.Value);
        }

        // 2. Filter by OccasionId
        if (request.OccasionId.HasValue && request.OccasionId.Value != Guid.Empty)
        {
            query = query.Where(p => p.ProductOccasions.Any(po => po.OccasionId == request.OccasionId.Value));
        }

        // 3. Search by Keyword (supports English and Arabic)
        string? trimmedKeyword = null;
        if (!string.IsNullOrWhiteSpace(request.Keyword))
        {
            trimmedKeyword = request.Keyword.Trim();
            query = query.Where(p => p.Name.Contains(trimmedKeyword) || (p.NameAr != null && p.NameAr.Contains(trimmedKeyword)));
        }

        var totalCount = await query.CountAsync(cancellationToken);

        // 4. Sort Order
        if (!string.IsNullOrWhiteSpace(request.SortBy))
        {
            query = request.SortBy.Trim().ToLowerInvariant() switch
            {
                "pricelowtohigh" => query.OrderBy(p => p.Price),
                "pricehightolow" => query.OrderByDescending(p => p.Price),
                "newest" => query.OrderByDescending(p => p.CreatedAt),
                "oldest" => query.OrderBy(p => p.CreatedAt),
                "discount" => query.OrderByDescending(p => p.DiscountPercentage ?? 0).ThenBy(p => p.Price),
                _ => ApplyDefaultSort(query, trimmedKeyword)
            };
        }
        else
        {
            query = ApplyDefaultSort(query, trimmedKeyword);
        }

        // 5. Pagination
        var page = request.Page > 0 ? request.Page : 1;
        var pageSize = request.PageSize > 0 ? request.PageSize : 10;

        var items = await query
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .Select(p => new ProductViewModel(
                p.Id,
                isArabic && !string.IsNullOrWhiteSpace(p.NameAr) ? p.NameAr : p.Name,
                p.ImageUrl,
                p.Currency,
                p.Price,
                p.OriginalPrice,
                p.DiscountPercentage,
                p.Status.ToString(),
                p.IsBestSeller
            ))
            .ToListAsync(cancellationToken);

        var pagedResult = new PagedResult<ProductViewModel>
        {
            Items = items,
            TotalCount = totalCount,
            PageNumber = page,
            PageSize = pageSize
        };

        return Result.Success(pagedResult);
    }

    private static IQueryable<Product> ApplyDefaultSort(IQueryable<Product> query, string? keyword)
    {
        if (!string.IsNullOrWhiteSpace(keyword))
        {
            return query
                .OrderByDescending(p => p.Name.StartsWith(keyword) || (p.NameAr != null && p.NameAr.StartsWith(keyword)))
                .ThenBy(p => p.Name);
        }

        return query.OrderByDescending(p => p.CreatedAt);
    }
}
