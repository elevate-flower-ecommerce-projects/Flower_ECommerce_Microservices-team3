using Blocks.Contracts.Common;
using Blocks.Contracts.Pagination;
using Catalog_Service.Features.Products.GetProductsByOccasionId.ViewModels;
using MediatR;

namespace Catalog_Service.Features.Products.GetProducts;

public record GetProductsQuery(
    Guid? OccasionId,
    Guid? CategoryId,
    string? Keyword,
    string? SortBy,
    string Language,
    int Page,
    int PageSize
) : IRequest<Result<PagedResult<ProductViewModel>>>;
