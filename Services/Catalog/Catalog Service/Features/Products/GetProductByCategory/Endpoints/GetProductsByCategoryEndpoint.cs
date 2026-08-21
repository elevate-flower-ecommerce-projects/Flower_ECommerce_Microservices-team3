using Catalog_Service.Features.Products.GetProductByCategory.Querry;
using MediatR;

namespace Catalog_Service.Features.Products.GetProductByCategory.Endpoints;

public static class GetProductsByCategoryEndpoint
{
    public static IEndpointRouteBuilder MapGetProductsByCategoryEndpoint(
        this IEndpointRouteBuilder endpoints)
    {
        endpoints.MapGet(
            "/products/by-category",
            async (
                Guid categoryId,
                IMediator mediator,
                CancellationToken cancellationToken) =>
            {
                var result = await mediator.Send(
                    new GetProductsByCategoryQuery(categoryId),
                    cancellationToken);

                return Results.Ok(result);
            })
            .WithName("GetProductsByCategory")
            .WithTags("Products");

        return endpoints;
    }
}