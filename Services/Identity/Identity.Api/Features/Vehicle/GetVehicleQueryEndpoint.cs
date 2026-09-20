using System.Security.Claims;
using Identity.Application.Features.Vehicle.GetVehicle;
using MediatR;

namespace Flower.Identity.Features.Drivers.Vehicle.GetVehicle;

public static class GetVehicleEndpoint
{
    public static IEndpointRouteBuilder MapGetVehicleEndpoint(
        this IEndpointRouteBuilder app)
    {
        app.MapGet("/api/v1/drivers/me/vehicle",
            async (
                ClaimsPrincipal user,
                ISender sender,
                CancellationToken cancellationToken) =>
            {
                var driverId = Guid.Parse(user.FindFirstValue(ClaimTypes.NameIdentifier)!);

                var result = await sender.Send(
                    new GetVehicleQuery(driverId),
                    cancellationToken);

                return result;
            })
            .WithTags("Vehicle")
            .RequireAuthorization();

        return app;
    }
}