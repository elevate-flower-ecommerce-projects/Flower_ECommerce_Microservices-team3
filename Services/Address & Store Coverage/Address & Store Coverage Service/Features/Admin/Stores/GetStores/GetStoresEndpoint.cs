using Address___Store_Coverage_Service.Features.Admin.Stores.GetStores.DTOs;
using Address___Store_Coverage_Service.Features.Admin.Stores.GetStores.Queries;
using Blocks.Contracts.Http;
using Blocks.Contracts.Security;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Address___Store_Coverage_Service.Features.Admin.Stores.GetStores
{
    public static class GetStoresEndpoint
    {
        public static IEndpointRouteBuilder MapGetStoresEndpoint(this IEndpointRouteBuilder app)
        {
            app.MapGet("/admin/stores", async (
                [FromQuery] int? page,
                [FromQuery] int? pageSize,
                IMediator mediator,
                CancellationToken ct) =>
            {
                var query = new GetStoresQuery(
                    Page: page ?? 1,
                    PageSize: pageSize ?? 10);

                var result = await mediator.Send(query, ct);

                if (result.IsFailure)
                {
                    return Results.Json(
                        ApiResponse<StoreListDto>.Fail(result.Error!),
                        statusCode: result.Error!.StatusCode);
                }

                return Results.Ok(ApiResponse<StoreListDto>.Ok(result.Value, "Stores retrieved successfully."));
            })
            .WithName("AdminGetStores")
            .WithTags("Admin - Store Coverage")
            .RequireAuthorization(FlowerClaimTypes.AdminPolicy);

            return app;
        }
    }
}
