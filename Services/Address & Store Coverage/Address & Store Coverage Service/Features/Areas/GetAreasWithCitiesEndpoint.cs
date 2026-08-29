using Address___Store_Coverage_Service.Features.Areas.DTOs;
using Address___Store_Coverage_Service.Features.Areas.Queries;
using Blocks.Contracts.Http;
using MediatR;

namespace Address___Store_Coverage_Service.Features.Areas
{
    public static class GetAreasWithCitiesEndpoint
    {
        public static IEndpointRouteBuilder MapGetAreasWithCitiesEndpoint(this IEndpointRouteBuilder app)
        {
            app.MapGet("/api/areas", async (IMediator mediator, CancellationToken ct) =>
            {
                var result = await mediator.Send(new GetAreasWithCitiesQuery(), ct);

                if (result.IsFailure)
                {
                    return Results.Json(
                        ApiResponse<List<AreaWithCitiesDto>>.Fail(result.Error!),
                        statusCode: result.Error!.StatusCode);
                }

                return Results.Ok(ApiResponse<List<AreaWithCitiesDto>>.Ok(result.Value));
            })
            .WithName("GetAreasWithCities")
            .WithTags("Lookups")
            .AllowAnonymous();

            return app;
        }
    }
}
