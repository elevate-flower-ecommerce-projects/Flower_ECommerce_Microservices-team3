using Blocks.Contracts.Common;
using Blocks.Contracts.Pagination;
using Catalog_Service.Features.Products.Queries.GetActivePromotions;
using Catalog_Service.Features.Products.Queries.GetProductStock;
using Catalog_Service.Features.Products.Queries.GetProductsPage;
using MediatR;

namespace Catalog_Service.Features.Products.Queries.GetProducts;

public sealed class GetProductsQueryOrchestratorHandler(ISender sender)
    : IRequestHandler<
        GetProductsQuery,
        Result<PagedResult<ProductSummaryResponse>>>
{
    public async Task<Result<PagedResult<ProductSummaryResponse>>> Handle(
        GetProductsQuery request,
        CancellationToken cancellationToken)
    {
        // 1. Get products page
        var productsResult = await sender.Send(
            new GetProductsPageQuery(
                request.Page,
                request.PageSize,
                request.CategoryId,
                request.OccasionId,
                request.StoreId),
            cancellationToken);

        if (productsResult.IsFailure)
            return productsResult.Error;

        var products = productsResult.Value.Items;

        // 2. Empty page
        if (products.Count == 0)
        {
            return Result.Success(
                PagedResult<ProductSummaryResponse>.Create(
                    [],
                    productsResult.Value.TotalCount,
                    new PaginationParams
                    {
                        PageNumber = productsResult.Value.PageNumber,
                        PageSize = productsResult.Value.PageSize
                    }));
        }

        // 3. Extract product IDs
        var productIds = products
            .Select(x => x.Id)
            .ToArray();

        // 4. Get active promotions
        var promotionsResult = await sender.Send(
            new GetActivePromotionsQuery(
                productIds,
                request.StoreId),
            cancellationToken);

        if (promotionsResult.IsFailure)
            return promotionsResult.Error;

        // 5. Get product stock
        var stockResult = await sender.Send(
            new GetProductStockQuery(productIds),
            cancellationToken);

        if (stockResult.IsFailure)
            return stockResult.Error;

        // 6. Create lookup dictionaries
        var promotions = promotionsResult.Value
            .ToDictionary(
                x => x.ProductId,
                x => x);

        var stock = stockResult.Value
            .ToDictionary(
                x => x.ProductId,
                x => x.InStock);

        // 7. Pricing + mapping
        var items = products
            .Select(product =>
            {
                decimal? discountedPrice = null;
                decimal? discountPercent = null;

                if (promotions.TryGetValue(
                    product.Id,
                    out var promotion))
                {
                    discountPercent = promotion.DiscountPercent;

                    discountedPrice = decimal.Round(
                        product.Price -
                        (product.Price *
                         promotion.DiscountPercent / 100m),
                        2,
                        MidpointRounding.AwayFromZero);
                }

                var inStock =
                    stock.TryGetValue(
                        product.Id,
                        out var stockStatus)
                    && stockStatus;

                return product with
                {
                    DiscountedPrice = discountedPrice,
                    DiscountPercent = discountPercent,
                    InStock = inStock
                };
            })
            .ToList();

        // 8. Create final paged result
        var result = PagedResult<ProductSummaryResponse>.Create(
            items,
            productsResult.Value.TotalCount,
            new PaginationParams
            {
                PageNumber = productsResult.Value.PageNumber,
                PageSize = productsResult.Value.PageSize
            });

        return Result.Success(result);
    }
}