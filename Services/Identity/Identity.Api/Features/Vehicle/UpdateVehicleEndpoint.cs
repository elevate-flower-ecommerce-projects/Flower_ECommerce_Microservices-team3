using System.Security.Claims;
using Blocks.Contracts.Http;
using Blocks.Domain.Errors;
using Identity.Application.Features.Vehicle.UpdateVehicle;
using MediatR;

namespace Identity.Api.Features.Vehicle
{
    public static class UpdateVehicleEndpoint
    {
        public static IEndpointRouteBuilder MapUpdateVehicleEndpoint(
            this IEndpointRouteBuilder app)
        {
            var handler = async (
                UpdateVehicleRequest request,
                ClaimsPrincipal user,
                ISender sender,
                CancellationToken cancellationToken) =>
            {
                var idClaim = user.FindFirstValue(ClaimTypes.NameIdentifier);
                if (string.IsNullOrEmpty(idClaim) || !Guid.TryParse(idClaim, out var driverId))
                {
                    return Results.Json(
                        ApiResponse<object>.Fail(Error.Unauthorized("You are not authorized to access this resource.")),
                        statusCode: StatusCodes.Status401Unauthorized);
                }

                var command = new UpdateVehicleCommand(
                    driverId,
                    request.VehicleType,
                    request.VehicleNumber);

                var result = await sender.Send(
                    command,
                    cancellationToken);

                if (result.IsSuccess)
                {
                    return Results.Ok(ApiResponse<bool>.Ok(true, "Vehicle updated successfully."));
                }

                return Results.Json(
                    ApiResponse<bool>.Fail(result.Error!),
                    statusCode: result.Error!.StatusCode == 0 ? StatusCodes.Status400BadRequest : result.Error.StatusCode);
            };

            app.MapPatch("/api/v1/drivers/me/vehicle", handler)
                .WithName("UpdateDriverVehicle")
                .WithTags("Vehicle")
                .WithSummary("Update Driver Vehicle")
                .WithDescription("Updates the vehicle type (1 = Car, 2 = Motorcycle) and vehicle plate number for the authenticated driver.")
                .Produces<ApiResponse<object>>(StatusCodes.Status200OK)
                .Produces<ApiResponse<object>>(StatusCodes.Status400BadRequest)
                .Produces<ApiResponse<object>>(StatusCodes.Status401Unauthorized)
                .Produces<ApiResponse<object>>(StatusCodes.Status404NotFound)
                .RequireAuthorization();

            app.MapPatch("/api/drivers/me/vehicle", handler).ExcludeFromDescription().RequireAuthorization();
            app.MapPatch("/drivers/me/vehicle", handler).ExcludeFromDescription().RequireAuthorization();

            return app;
        }
    }
}

