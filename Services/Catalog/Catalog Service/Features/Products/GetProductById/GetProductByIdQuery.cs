using Blocks.Contracts.Common;
using MediatR;

namespace Catalog_Service.Features.Products.GetProductById;

public sealed record GetProductByIdQuery(Guid ProductId, string Language)
    : IRequest<Result<ProductDetailsDto>>;
