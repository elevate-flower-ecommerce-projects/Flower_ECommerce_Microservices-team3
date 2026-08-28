using Address___Store_Coverage_Service.Features.Admin.Stores.Common.DTOs;
using Address___Store_Coverage_Service.Features.Admin.Stores.UpdateStore.Commands;
using Address___Store_Coverage_Service.Features.Admin.Stores.UpdateStore.DTOs;
using Blocks.Contracts.Http;
using Blocks.Contracts.Security;
using MediatR;

namespace Address___Store_Coverage_Service.Features.Admin.Stores.UpdateStore
{
    public static class UpdateStoreEndpoint
    {
        public static IEndpointRouteBuilder MapUpdateStoreEndpoint(this IEndpointRouteBuilder app)
        {
            app.MapPut("/admin/stores/{id:guid}", async (
                Guid id,
                StoreUpdateRequestDto request,
                IMediator mediator,
                CancellationToken ct) =>
            {
                var command = new UpdateStoreCommand(
                    Id: id,
                    Name: request.Name,
                    Location: request.Location,
                    IsActive: request.IsActive);

                var result = await mediator.Send(command, ct);

                if (result.IsFailure)
                {
                    return Results.Json(
                        ApiResponse<StoreDto>.Fail(result.Error!),
                        statusCode: result.Error!.StatusCode);
                }

                return Results.Ok(ApiResponse<StoreDto>.Ok(result.Value, "Store updated successfully."));
            })
            .WithName("AdminUpdateStore")
            .WithTags("Admin - Store Coverage")
            .RequireAuthorization(FlowerClaimTypes.AdminPolicy);

            return app;
        }
    }
}
