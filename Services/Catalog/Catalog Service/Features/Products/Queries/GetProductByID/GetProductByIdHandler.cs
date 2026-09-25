using Blocks.Contracts.Common;
using Blocks.Domain.Errors;
using Catalog_Service.Entities;
using Catalog_Service.Features.Products.Queries.GetProductByID;
using Catalog_Service.Features.Products.Queries.GetProducts;
using Catalog_Service.Persistence.Repositories.Interfaces;
using MediatR;

namespace Catalog_Service.Features.Products.Queries.GetProductById;

    public sealed class GetProductByIdHandler(IProductRepository productRepository)
        : IRequestHandler<GetProductByIdQuery, Result<ProductSummaryResponse>>
    {
        public async Task<Result<ProductSummaryResponse>> Handle(
            GetProductByIdQuery request,
            CancellationToken cancellationToken)
        {
            var product = await productRepository.GetByIdAsync(
                request.Id,
                cancellationToken);

            if (product is null)
                return Result.Failure<ProductSummaryResponse>(
                    Error.NotFound("Product not found"));

            var response = new ProductSummaryResponse(
                product.Id,
                product.Name,
                product.ImageUrl,
                product.Price,
                product.OriginalPrice,
                product.DiscountPercentage,
                product.Status == ProductStatus.InStock);

            return response;
        }
    }
