using System.Globalization;
using Blocks.Contracts.Common;
using Blocks.Domain.Errors;
using Catalog_Service.Entities;
using Catalog_Service.Features.Products.Queries.GetProductByID;
using Catalog_Service.Persistence.Repositories.Interfaces;
using MediatR;

namespace Catalog_Service.Features.Products.Queries.GetProductById;

public sealed class GetProductByIdHandler(
    IProductRepository productRepository,
    IInventoryRepository inventoryRepository)
    : IRequestHandler<GetProductByIdQuery, Result<ProductDetailsDto>>
{
    public async Task<Result<ProductDetailsDto>> Handle(
        GetProductByIdQuery request,
        CancellationToken cancellationToken)
    {
        var product = await productRepository.GetProductDetailsByIdAsync(
            request.Id,
            cancellationToken);

        if (product is null)
            return Result.Failure<ProductDetailsDto>(
                Error.NotFound("Product not found"));

        var isArabic = CultureInfo.CurrentUICulture.Name.StartsWith("ar");

        // 1. Stock Status
        var stock = await inventoryRepository.GetProductsStockAsync([product.Id], cancellationToken);
        var inStock = stock.FirstOrDefault()?.InStock ?? (product.Status == ProductStatus.InStock);
        var status = (product.Status == ProductStatus.InStock && inStock)
            ? ProductStatus.InStock.ToString()
            : ProductStatus.OutOfStock.ToString();

        // 2. Localized Name & Description
        var name = (isArabic && !string.IsNullOrWhiteSpace(product.NameAr))
            ? product.NameAr
            : product.Name;

        var description = (isArabic && !string.IsNullOrWhiteSpace(product.DescriptionAr))
            ? product.DescriptionAr
            : (product.Description ?? string.Empty);

        // 3. Gallery Images (including main ImageUrl)
        var images = product.Images?
            .OrderBy(i => i.SortOrder)
            .Select(i => i.ImageUrl)
            .Where(url => !string.IsNullOrWhiteSpace(url))
            .ToList() ?? [];

        if (!string.IsNullOrWhiteSpace(product.ImageUrl) && !images.Contains(product.ImageUrl))
        {
            images.Insert(0, product.ImageUrl);
        }

        // 4. Includes
        var includes = product.Includes?
            .Select(inc => new ProductIncludeDto
            {
                Name = (isArabic && !string.IsNullOrWhiteSpace(inc.NameAr)) ? inc.NameAr : inc.Name
            })
            .Where(inc => !string.IsNullOrWhiteSpace(inc.Name))
            .ToList() ?? [];

        // 5. Pricing and Discount
        var originalPrice = product.OriginalPrice ?? 0;
        var discountPercentage = product.DiscountPercentage ?? 0;

        if (originalPrice > 0 && discountPercentage == 0 && product.Price < originalPrice)
        {
            discountPercentage = (int)Math.Round((1 - (product.Price / originalPrice)) * 100);
        }

        var response = new ProductDetailsDto
        {
            Id = product.Id,
            Name = name,
            ImageUrl = product.ImageUrl,
            Currency = string.IsNullOrWhiteSpace(product.Currency) ? "EGP" : product.Currency,
            Price = product.Price,
            OriginalPrice = originalPrice,
            DiscountPercentage = discountPercentage,
            Status = status,
            Images = images,
            Description = description,
            Includes = includes
        };

        return Result.Success(response);
    }
}
