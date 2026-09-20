using Address___Store_Coverage_Service.Features.NearestCoveringStore.DTOs;
using Address___Store_Coverage_Service.Features.NearestCoveringStore.Queries;
using Blocks.Contracts.Http;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Address___Store_Coverage_Service.Features.NearestCoveringStore
{
    public static class FindNearestCoveringStoreEndpoint
    {
        public static IEndpointRouteBuilder MapFindNearestCoveringStoreEndpoint(this IEndpointRouteBuilder app)
        {
            app.MapGet("/api/stores/nearest", async (
                [FromQuery] double latitude,
                [FromQuery] double longitude,
                IMediator mediator,
                CancellationToken ct) =>
            {
                var result = await mediator.Send(new FindNearestCoveringStoreQuery(latitude, longitude), ct);

                if (result.IsFailure)
                {
                    return Results.Json(
                        ApiResponse<NearestStoreDto>.Fail(result.Error!),
                        statusCode: result.Error!.StatusCode);
                }

                return Results.Ok(ApiResponse<NearestStoreDto>.Ok(result.Value));
            })
            .WithName("FindNearestCoveringStore")
            .WithTags("Stores")
            .AllowAnonymous();

            return app;
        }
    }
}
