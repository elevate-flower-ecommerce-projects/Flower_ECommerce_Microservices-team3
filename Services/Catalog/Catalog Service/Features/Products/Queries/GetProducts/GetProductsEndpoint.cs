using MediatR;

namespace Catalog_Service.Features.Products.Queries.GetProducts
{
    public static class GetProductsEndpoint
    {
        public static void MapGetProductsEndpoint(this IEndpointRouteBuilder app)
        {
            app.MapGet("/products", async (
                [AsParameters] GetProductsRequest request,
                ISender _sender,
                CancellationToken cancellationToken) =>
            {
                var query = new GetProductsQuery(
                    request.Page,
                    request.PageSize,
                    request.CategoryId,
                    request.OccasionId,
                    request.StoreId);

                var result = await _sender.Send(query, cancellationToken);

                return result;
            })
        .WithName("GetProducts")
        .WithTags("Products")
        .WithSummary("Get products")
        .WithDescription("Returns a paginated list of products with pricing, discount, and stock information.")
        .Produces<GetProductsResponse>(StatusCodes.Status200OK)
        .ProducesProblem(StatusCodes.Status400BadRequest)
        .ProducesProblem(StatusCodes.Status500InternalServerError);
        }
    }
}
