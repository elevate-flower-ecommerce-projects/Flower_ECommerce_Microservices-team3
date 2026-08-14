using Blocks.Contracts.Common;
using Blocks.Contracts.Interfaces;
using Blocks.Domain.Errors;
using Catalog_Service.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Catalog_Service.Features.Products.GetProductById;

public sealed class GetProductByIdHandler : IRequestHandler<GetProductByIdQuery, Result<ProductDetailsDto>>
{
    private readonly IGenericRepository<Product> _productRepo;

    public GetProductByIdHandler(IGenericRepository<Product> productRepo)
    {
        _productRepo = productRepo;
    }

    public async Task<Result<ProductDetailsDto>> Handle(GetProductByIdQuery query, CancellationToken ct)
    {
        var isArabic = query.Language == "ar";

        var product = await _productRepo
            .GetQueryable()
            .AsNoTracking()
            .Where(p => p.Id == query.ProductId)
            .Select(p => new ProductDetailsDto
            {
                Id = p.Id,
                Name = isArabic ? (p.NameAr ?? p.Name) : p.Name,
                ImageUrl = p.ImageUrl,
                Currency = p.Currency,
                Price = p.Price,
                OriginalPrice = p.OriginalPrice,
                DiscountPercentage = p.DiscountPercentage,
                Status = p.Status.ToString(),
                Images = p.Images
                    .OrderBy(i => i.SortOrder)
                    .Select(i => i.ImageUrl)
                    .ToList(),
                Description = isArabic
                    ? (p.DescriptionAr ?? p.Description ?? string.Empty)
                    : (p.Description ?? string.Empty),
                Includes = p.Includes
                    .Select(i => new ProductIncludeDto(isArabic ? (i.NameAr ?? i.Name) : i.Name))
                    .ToList(),
                CategoryId = p.CategoryId,
                OccasionIds = p.ProductOccasions
                    .Select(po => po.OccasionId)
                    .ToList()
            })
            .FirstOrDefaultAsync(ct);

        if (product is null)
        {
            return Error.NotFound("Product not found");
        }

        return Result.Success(product);
    }
}
