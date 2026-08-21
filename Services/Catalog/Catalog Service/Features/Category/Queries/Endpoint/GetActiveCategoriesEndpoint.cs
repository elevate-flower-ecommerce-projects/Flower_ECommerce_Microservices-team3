using Catalog_Service.Features.Categories.Queries.GetActiveCategories;
using MediatR;

namespace Catalog_Service.Features.Categories.GetActiveCategories.Endpoints;

public static class GetActiveCategoriesEndpoint
{
    public static IEndpointRouteBuilder MapGetActiveCategoriesEndpoint(
        this IEndpointRouteBuilder endpoints)
    {
        endpoints.MapGet(
            "/categories",
            async (
                IMediator mediator,
                CancellationToken cancellationToken) =>
            {
                var result = await mediator.Send(
                    new GetActiveCategoriesQuery(),
                    cancellationToken);

                return Results.Ok(result);
            })
        .WithName("GetActiveCategories")
        .WithTags("Categories");

        return endpoints;
    }
}