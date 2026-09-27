using System.Security.Claims;
using Identity.Application.Features.Vehicle.UpdateVehicle;
using Identity.Domain.Enums;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Identity.Api.Features.Vehicle;

public static class UpdateVehicleEndpoint
{
    public static IEndpointRouteBuilder MapUpdateVehicleEndpoint(
        this IEndpointRouteBuilder app)
    {
        app.MapPatch("/api/v1/drivers/me/vehicle",
            async (
                [FromForm] VehicleType vehicleType,
                [FromForm] string vehicleNumber,
                IFormFile licenseDocument,
                ClaimsPrincipal user,
                ISender sender,
                CancellationToken cancellationToken) =>
            {
                var driverId = Guid.Parse(
                    user.FindFirstValue(ClaimTypes.NameIdentifier)!);

                var command = new UpdateVehicleCommand(
                    driverId,
                    vehicleType,
                    vehicleNumber,
                    licenseDocument);

                return await sender.Send(
                    command,
                    cancellationToken);
            })
            .DisableAntiforgery()
            .WithTags("Vehicle")
            .RequireAuthorization("Driver");

        return app;
    }
}