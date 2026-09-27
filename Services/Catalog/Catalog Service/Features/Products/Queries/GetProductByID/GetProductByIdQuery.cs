using Blocks.Contracts.Common;
using Catalog_Service.Features.Products.Queries.GetProducts;
using MediatR;

namespace Catalog_Service.Features.Products.Queries.GetProductByID
{
    public sealed record GetProductByIdQuery(Guid Id)
        : IRequest<Result<ProductDetailsDto>>;
}
