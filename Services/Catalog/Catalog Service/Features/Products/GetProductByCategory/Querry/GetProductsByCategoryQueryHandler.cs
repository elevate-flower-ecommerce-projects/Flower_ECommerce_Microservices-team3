using Blocks.Contracts.Common;
using Blocks.Contracts.Interfaces;
using Catalog_Service.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Catalog_Service.Features.Products.GetProductByCategory.Querry;

public sealed class GetProductsByCategoryQueryHandler
    : IRequestHandler<GetProductsByCategoryQuery, Result<List<ProductDto>>>
{
    private readonly IGenericRepository<Product> _productRepo;
    private readonly IGenericRepository<Category> _categoryRepo;

    public GetProductsByCategoryQueryHandler(
        IGenericRepository<Product> productRepo,
        IGenericRepository<Category> categoryRepo)
    {
        _productRepo = productRepo;
        _categoryRepo = categoryRepo;
    }

    public async Task<Result<List<ProductDto>>> Handle(
        GetProductsByCategoryQuery query,
        CancellationToken ct)
    {
        var category = await _categoryRepo
            .GetQueryable()
            .AsNoTracking()
            .FirstOrDefaultAsync(
                c => c.Id == query.CategoryId,
                ct);

        if (category is null || !category.IsActive)
        {
            return Error.NotFound("Category is no longer available");
        }

        var isArabic = query.Language == "ar";

        var products = await _productRepo
            .GetQueryable()
            .AsNoTracking()
            .Where(p => p.CategoryId == query.CategoryId)
            .Select(p => new ProductDto
            {
                Id = p.Id,

                Name = isArabic
                    ? (p.NameAr ?? p.Name)
                    : p.Name,

                ImageUrl = p.ImageUrl,

                Currency = p.Currency,

                Price = p.Price,

                OriginalPrice = p.OriginalPrice,

                DiscountPercentage = p.DiscountPercentage,

                Status = p.Status.ToString(),

                CategoryId = p.CategoryId
            })
            .ToListAsync(ct);

        return Result.Success(products);
    }
}