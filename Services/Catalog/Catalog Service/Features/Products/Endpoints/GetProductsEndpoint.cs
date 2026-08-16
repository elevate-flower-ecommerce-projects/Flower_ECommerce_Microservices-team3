using Catalog_Service.Features.Products.Queries;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Catalog_Service.Features.Products.Endpoints
{
    public static class GetProductsEndpoint
    {
        public static void MapGetProductsEndpoint(this IEndpointRouteBuilder app)
        {
            app.MapGet("/products", async (
                int? pageNumber,
                int? pageSize,
                Guid? occasionId,
                [FromServices] ISender sender,
                CancellationToken cancellationToken) =>
            {
                var query = new GetProductsQuery(pageNumber ?? 1, pageSize ?? 10, occasionId);

                var result = await sender.Send(query, cancellationToken);

                if (result.IsSuccess)
                {
                    return Results.Ok(result);
                }

                return Results.BadRequest(result);
            })
            .WithName("GetProducts")
            .WithTags("Products")
            .Produces(StatusCodes.Status200OK)
            .WithSummary("Get a paginated list of products")
            .WithDescription("Retrieves products. Can be optionally filtered by occasionId.");
        }
    }
}
