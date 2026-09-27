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
            var handler = async (
                [FromQuery] double? latitude,
                [FromQuery] double? longitude,
                [FromQuery] double? lat,
                [FromQuery] double? lng,
                IMediator mediator,
                CancellationToken ct) =>
            {
                var resolvedLat = latitude ?? lat ?? 0;
                var resolvedLng = longitude ?? lng ?? 0;

                var result = await mediator.Send(new FindNearestCoveringStoreQuery(resolvedLat, resolvedLng), ct);

                if (result.IsFailure)
                {
                    return Results.Json(
                        ApiResponse<NearestStoreDto>.Fail(result.Error!),
                        statusCode: result.Error!.StatusCode);
                }

                return Results.Ok(ApiResponse<NearestStoreDto>.Ok(result.Value));
            };

            app.MapGet("/api/stores/nearest", handler)
                .WithName("FindNearestCoveringStore")
                .WithTags("Stores")
                .AllowAnonymous();

            app.MapGet("/api/stores/nearest-store", handler).AllowAnonymous().ExcludeFromDescription();
            app.MapGet("/stores/nearest", handler).AllowAnonymous().ExcludeFromDescription();
            app.MapGet("/stores/nearest-store", handler).AllowAnonymous().ExcludeFromDescription();

            return app;
        }
    }
}
