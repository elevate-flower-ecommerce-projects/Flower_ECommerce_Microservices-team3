using Address___Store_Coverage_Service.Features.Admin.Stores.DeleteStore.Commands;
using Blocks.Contracts.Http;
using Blocks.Contracts.Security;
using MediatR;

namespace Address___Store_Coverage_Service.Features.Admin.Stores.DeleteStore
{
    public static class DeleteStoreEndpoint
    {
        public static IEndpointRouteBuilder MapDeleteStoreEndpoint(this IEndpointRouteBuilder app)
        {
            app.MapDelete("/admin/stores/{id:guid}", async (
                Guid id,
                IMediator mediator,
                CancellationToken ct) =>
            {
                var result = await mediator.Send(new DeleteStoreCommand(id), ct);

                if (result.IsFailure)
                {
                    return Results.Json(
                        ApiResponse<string>.Fail(result.Error!),
                        statusCode: result.Error!.StatusCode);
                }

                return Results.Ok(ApiResponse<string>.Ok(result.Value, "Store deactivated successfully."));
            })
            .WithName("AdminDeleteStore")
            .WithTags("Admin - Store Coverage")
            .RequireAuthorization(FlowerClaimTypes.AdminPolicy);

            return app;
        }
    }
}
