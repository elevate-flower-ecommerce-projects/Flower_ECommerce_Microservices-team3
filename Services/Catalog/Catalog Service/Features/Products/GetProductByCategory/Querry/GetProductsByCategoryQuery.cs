using Blocks.Contracts.Common;
using MediatR;

namespace Catalog_Service.Features.Products.GetProductByCategory.Querry;

public sealed record GetProductsByCategoryQuery(
    Guid CategoryId,
    string? Language = "en"
) : IRequest<Result<List<ProductDto>>>;