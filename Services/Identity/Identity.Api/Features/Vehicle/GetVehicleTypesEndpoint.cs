using MediatR;

namespace Identity.Application.Features.VehicleTypes.GetVehicleTypes;

public static class GetVehicleTypesEndpoint
{
    public static IEndpointRouteBuilder MapGetVehicleTypesEndpoint(
        this IEndpointRouteBuilder app)
    {
        app.MapGet( "/api/v1/vehicle-types",
            async (
                ISender sender,
                CancellationToken cancellationToken) =>
            {
                var result = await sender.Send(
                    new GetVehicleTypesQuery(),
                    cancellationToken);

                return result;
            })
            .WithTags("Vehicle");

        return app;
    }
}