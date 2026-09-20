using System.Security.Claims;
using Identity.Application.Features.Vehicle.UpdateVehicle;
using MediatR;

namespace Identity.Api.Features.Vehicle
{
    public static class UpdateVehicleEndpoint
    {
        public static IEndpointRouteBuilder MapUpdateVehicleEndpoint(
            this IEndpointRouteBuilder app)
        {
            app.MapPatch("/api/v1/drivers/me/vehicle",
                async (
                    UpdateVehicleRequest request,
                    ClaimsPrincipal user,
                    ISender sender,
                    CancellationToken cancellationToken) =>
                {
                    var driverId = Guid.Parse(
                        user.FindFirstValue(
                            ClaimTypes.NameIdentifier)!);

                    var command = new UpdateVehicleCommand(
                        driverId,
                        request.VehicleType,
                        request.VehicleNumber);

                    var result = await sender.Send(
                        command,
                        cancellationToken);

                    return result;
                })
                .WithTags("Vehicle")
                .RequireAuthorization();

            return app;
        }
    }
}
