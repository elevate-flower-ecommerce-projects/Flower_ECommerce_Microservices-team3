namespace Catalog_Service.Features.Products.Queries.GetProducts
{
    public sealed record GetProductsResponse(
        IReadOnlyCollection<ProductSummaryResponse> Items,
        int Page,
        int PageSize,
        int TotalCount,
        bool HasNextPage
    );
}
