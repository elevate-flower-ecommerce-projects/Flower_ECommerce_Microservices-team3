using Address___Store_Coverage_Service.Features.Admin.Stores.Common.DTOs;
using Address___Store_Coverage_Service.Features.Admin.Stores.CreateStore.Commands;
using Address___Store_Coverage_Service.Features.Admin.Stores.CreateStore.DTOs;
using Blocks.Contracts.Http;
using Blocks.Contracts.Security;
using Blocks.Domain.Errors;
using MediatR;

namespace Address___Store_Coverage_Service.Features.Admin.Stores.CreateStore
{
    public static class CreateStoreEndpoint
    {
        public static IEndpointRouteBuilder MapCreateStoreEndpoint(this IEndpointRouteBuilder app)
        {
            app.MapPost("/admin/stores", async (
                StoreCreateRequestDto request,
                IMediator mediator,
                CancellationToken ct) =>
            {
                if (request.Location is null)
                {
                    return Results.Json(
                        ApiResponse<StoreDto>.Fail(
                            Error.Validation("Location is required.", "location")),
                        statusCode: 422);
                }

                var command = new CreateStoreCommand(
                    Name: request.Name,
                    Latitude: request.Location.Lat,
                    Longitude: request.Location.Lng,
                    IsActive: request.IsActive);

                var result = await mediator.Send(command, ct);

                if (result.IsFailure)
                {
                    return Results.Json(
                        ApiResponse<StoreDto>.Fail(result.Error!),
                        statusCode: result.Error!.StatusCode);
                }

                return Results.Json(
                    ApiResponse<StoreDto>.Ok(result.Value, "Store created successfully."),
                    statusCode: StatusCodes.Status201Created);
            })
            .WithName("AdminCreateStore")
            .WithTags("Admin - Store Coverage")
            .RequireAuthorization(FlowerClaimTypes.AdminPolicy);

            return app;
        }
    }
}
