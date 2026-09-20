using Blocks.Contracts.Common;
using MediatR;

namespace Catalog_Service.Features.Products.Queries.GetProductStock
{
    public sealed record GetProductStockQuery(
        IReadOnlyCollection<Guid> ProductIds) 
        : IRequest<Result<IReadOnlyCollection<ProductStockResponse>>>;
}
