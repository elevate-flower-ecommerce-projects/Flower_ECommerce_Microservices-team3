using Address___Store_Coverage_Service.Features.Admin.Stores.Common.DTOs;
using Address___Store_Coverage_Service.Features.Admin.Stores.GetStoreById.Queries;
using Blocks.Contracts.Http;
using MediatR;

namespace Address___Store_Coverage_Service.Features.InternalStores;

public static class GetStoreInternalEndpoint
{
    public static IEndpointRouteBuilder MapGetStoreInternalEndpoint(this IEndpointRouteBuilder app)
    {
        app.MapGet("/internal/stores/{id:guid}", async (
            Guid id,
            IMediator mediator,
            CancellationToken ct) =>
        {
            var result = await mediator.Send(new GetStoreByIdQuery(id), ct);

            if (result.IsFailure)
            {
                return Results.Json(
                    ApiResponse<StoreDto>.Fail(result.Error!),
                    statusCode: result.Error!.StatusCode);
            }

            return Results.Ok(ApiResponse<StoreDto>.Ok(result.Value));
        })
        .WithName("InternalGetStoreById")
        .WithTags("Internal")
        .AllowAnonymous();

        return app;
    }
}
