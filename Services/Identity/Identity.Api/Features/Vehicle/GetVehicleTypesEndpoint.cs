using Blocks.Contracts.Http;
using Identity.Application.Features.Vehicle.VehicleTypes.GetVehicleTypes;
using MediatR;

namespace Identity.Application.Features.VehicleTypes.GetVehicleTypes;

public static class GetVehicleTypesEndpoint
{
    public static IEndpointRouteBuilder MapGetVehicleTypesEndpoint(
        this IEndpointRouteBuilder app)
    {
        var handler = async (
            ISender sender,
            CancellationToken cancellationToken) =>
        {
            var result = await sender.Send(
                new GetVehicleTypesQuery(),
                cancellationToken);

            if (result.IsSuccess)
            {
                return Results.Ok(ApiResponse<List<VehicleTypeResponse>>.Ok(result.Value));
            }

            return Results.Json(
                ApiResponse<List<VehicleTypeResponse>>.Fail(result.Error!),
                statusCode: result.Error!.StatusCode == 0 ? StatusCodes.Status400BadRequest : result.Error.StatusCode);
        };

        app.MapGet("/api/v1/vehicle-types", handler)
            .WithName("GetVehicleTypes")
            .WithTags("Vehicle")
            .WithSummary("Get Vehicle Types")
            .WithDescription("Returns available vehicle types (1 = Car, 2 = Motorcycle) for drivers.")
            .Produces<ApiResponse<List<VehicleTypeResponse>>>(StatusCodes.Status200OK);

        app.MapGet("/api/vehicle-types", handler).ExcludeFromDescription();
        app.MapGet("/vehicle-types", handler).ExcludeFromDescription();

        return app;
    }
}