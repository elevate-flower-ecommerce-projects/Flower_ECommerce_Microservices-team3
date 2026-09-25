using Catalog_Service.Features.Products.Queries.GetProducts;
using MediatR;

namespace Catalog_Service.Features.Products.Queries.GetProductByID
{
    public static class GetProductByIdEndpoint
    {
        public static IEndpointRouteBuilder MapGetProductByIdEndpoint(
            this IEndpointRouteBuilder app)
        {
            var handler = async (Guid id,
                   ISender sender,
                   CancellationToken cancellationToken) =>
            {
                var result = await sender.Send(
                    new GetProductByIdQuery(id),
                    cancellationToken);

                return result;
            };

            app.MapGet("/products/{id:guid}", handler)
                .WithName("GetProductById")
                .WithTags("Products")
                .WithSummary("Get Product by ID")
                .WithDescription("Retrieves details, pricing, discount, and stock status for a single product.")
                .Produces<ProductSummaryResponse>(StatusCodes.Status200OK)
                .ProducesProblem(StatusCodes.Status404NotFound);

            app.MapGet("/api/v1/products/{id:guid}", handler).ExcludeFromDescription();
            app.MapGet("/api/products/{id:guid}", handler).ExcludeFromDescription();

            return app;
        }
    }
}

