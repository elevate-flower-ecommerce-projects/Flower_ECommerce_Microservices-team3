using System.Security.Claims;
using Blocks.Contracts.Http;
using Blocks.Domain.Errors;
using Identity.Application.Features.Vehicle.GetVehicle;
using MediatR;

namespace Flower.Identity.Features.Drivers.Vehicle.GetVehicle;

public static class GetVehicleEndpoint
{
    public static IEndpointRouteBuilder MapGetVehicleEndpoint(
        this IEndpointRouteBuilder app)
    {
        var handler = async (
            ClaimsPrincipal user,
            ISender sender,
            CancellationToken cancellationToken) =>
        {
            var idClaim = user.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(idClaim) || !Guid.TryParse(idClaim, out var driverId))
            {
                return Results.Json(
                    ApiResponse<GetVehicleResponse>.Fail(Error.Unauthorized("You are not authorized to access this resource.")),
                    statusCode: StatusCodes.Status401Unauthorized);
            }

            var result = await sender.Send(
                new GetVehicleQuery(driverId),
                cancellationToken);

            if (result.IsSuccess)
            {
                return Results.Ok(ApiResponse<GetVehicleResponse>.Ok(result.Value));
            }

            return Results.Json(
                ApiResponse<GetVehicleResponse>.Fail(result.Error!),
                statusCode: result.Error!.StatusCode == 0 ? StatusCodes.Status400BadRequest : result.Error.StatusCode);
        };

        app.MapGet("/api/v1/drivers/me/vehicle", handler)
            .WithName("GetDriverVehicle")
            .WithTags("Vehicle")
            .WithSummary("Get Driver Vehicle")
            .WithDescription("Retrieves the vehicle type and vehicle number for the authenticated driver.")
            .Produces<ApiResponse<GetVehicleResponse>>(StatusCodes.Status200OK)
            .Produces<ApiResponse<GetVehicleResponse>>(StatusCodes.Status401Unauthorized)
            .Produces<ApiResponse<GetVehicleResponse>>(StatusCodes.Status404NotFound)
            .RequireAuthorization();

        app.MapGet("/api/drivers/me/vehicle", handler).ExcludeFromDescription().RequireAuthorization();
        app.MapGet("/drivers/me/vehicle", handler).ExcludeFromDescription().RequireAuthorization();

        return app;
    }
}