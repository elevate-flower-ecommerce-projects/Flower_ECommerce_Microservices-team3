using Blocks.Contracts.Http;
using MediatR;

namespace Catalog_Service.Features.Home.GetSections;

public static class GetHomeSectionsEndpoint
{
    public static IEndpointRouteBuilder MapGetHomeSectionsEndpoint(this IEndpointRouteBuilder app)
    {
        app.MapGet("/home/sections", async (
            IMediator mediator,
            CancellationToken cancellationToken) =>
        {
            var result = await mediator.Send(new GetHomeSectionsQuery(), cancellationToken);

            if (result.IsFailure)
            {
                return Results.Json(
                    ApiResponse<List<HomeSectionResponse>>.Fail(result.Error!),
                    statusCode: result.Error!.StatusCode);
            }

            return Results.Ok(
                ApiResponse<List<HomeSectionResponse>>.Ok(result.Value));
        })
        .WithTags("Home")
        .WithName("GetHomeSections")
        .Produces<ApiResponse<List<HomeSectionResponse>>>(StatusCodes.Status200OK)
        .Produces(StatusCodes.Status500InternalServerError);

        return app;
    }
}
