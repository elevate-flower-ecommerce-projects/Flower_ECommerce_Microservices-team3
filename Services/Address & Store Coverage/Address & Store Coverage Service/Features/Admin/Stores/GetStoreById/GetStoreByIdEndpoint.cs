using Address___Store_Coverage_Service.Features.Admin.Stores.Common.DTOs;
using Address___Store_Coverage_Service.Features.Admin.Stores.GetStoreById.Queries;
using Blocks.Contracts.Http;
using Blocks.Contracts.Security;
using MediatR;

namespace Address___Store_Coverage_Service.Features.Admin.Stores.GetStoreById
{
    public static class GetStoreByIdEndpoint
    {
        public static IEndpointRouteBuilder MapGetStoreByIdEndpoint(this IEndpointRouteBuilder app)
        {
            app.MapGet("/admin/stores/{id:guid}", async (
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
            .WithName("AdminGetStoreById")
            .WithTags("Admin - Store Coverage")
            .RequireAuthorization(FlowerClaimTypes.AdminPolicy);

            return app;
        }
    }
}
