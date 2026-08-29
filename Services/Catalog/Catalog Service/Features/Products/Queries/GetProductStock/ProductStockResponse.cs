namespace Catalog_Service.Features.Products.Queries.GetProductStock
{
    public sealed record ProductStockResponse(
        Guid ProductId,
        bool InStock
    );
}
