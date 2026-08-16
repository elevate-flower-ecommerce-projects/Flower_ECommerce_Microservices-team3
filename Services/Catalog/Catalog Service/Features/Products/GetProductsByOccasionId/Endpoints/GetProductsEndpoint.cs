using Catalog_Service.Features.Products.GetProductsByOccasionId.Queries;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Catalog_Service.Features.Products.GetProductsByOccasionId.Endpoints
{
    public static class GetProductsEndpoint
    {
        public static void MapGetProductsEndpoint(this IEndpointRouteBuilder app)
        {
            app.MapGet("/products", async (
                Guid occasionId,
                int? pageNumber,
                int? pageSize,
                [FromServices] ISender sender,
                CancellationToken cancellationToken) =>
            {
                var query = new GetProductsQuery(occasionId, pageNumber ?? 1, pageSize ?? 10);

                var result = await sender.Send(query, cancellationToken);

                if (result.IsSuccess)
                {
                    return Results.Ok(result);
                }

                return Results.BadRequest(result);
            })
            .WithName("GetProductsByOccasion")
            .WithTags("Products")
            .Produces(StatusCodes.Status200OK)
            .Produces(StatusCodes.Status400BadRequest)
            .WithSummary("Get a paginated list of products for a specific occasion")
            .WithDescription("Retrieves products filtered by the required occasionId. Returns an empty list if no products are found.");
        }
    }
}
