using Address___Store_Coverage_Service.Features.Cities.DTOs;
using Address___Store_Coverage_Service.Features.Cities.Queries;
using Blocks.Contracts.Http;
using MediatR;

namespace Address___Store_Coverage_Service.Features.Cities
{
    public static class GetCitiesWithAreasEndpoint
    {
        public static IEndpointRouteBuilder MapGetCitiesWithAreasEndpoint(this IEndpointRouteBuilder app)
        {
            app.MapGet("/api/cities", async (IMediator mediator, CancellationToken ct) =>
            {
                var result = await mediator.Send(new GetCitiesWithAreasQuery(), ct);

                if (result.IsFailure)
                {
                    return Results.Json(
                        ApiResponse<List<CityWithAreasDto>>.Fail(result.Error!),
                        statusCode: result.Error!.StatusCode);
                }

                return Results.Ok(ApiResponse<List<CityWithAreasDto>>.Ok(result.Value));
            })
            .WithName("GetCitiesWithAreas")
            .WithTags("Lookups")
            .AllowAnonymous();

            return app;
        }
    }
}
