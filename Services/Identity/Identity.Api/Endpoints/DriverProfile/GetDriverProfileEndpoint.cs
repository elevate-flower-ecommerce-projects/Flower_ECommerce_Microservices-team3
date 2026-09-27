using Blocks.Contracts.Http;
using Blocks.Domain.Errors;
using Identity.Application.DTOs;
using Identity.Application.Features.Drivers.Queries;
using MediatR;

namespace Identity.Api.Features.DriverProfile;

public static class GetDriverProfileEndpoint
{
    public static IEndpointRouteBuilder MapGetDriverProfileEndpoint(this IEndpointRouteBuilder app)
    {
        app.MapGet("/api/drivers/{driverId:guid}/profile", async (
            Guid driverId,
            IMediator mediator,
            CancellationToken cancellationToken) =>
        {
            var profile = await mediator.Send(
                new GetDriverProfileQuery(driverId), cancellationToken);

            if (profile == null)
            {
                return Results.Json(
                    ApiResponse<DriverProfileResponse>.Fail(
                        Error.NotFound("Driver profile not found.")),
                    statusCode: StatusCodes.Status404NotFound);
            }

            return Results.Ok(ApiResponse<DriverProfileResponse>.Ok(profile));
        })
        .WithName("GetDriverProfile")
        .WithTags("Drivers")
        .WithSummary("Get Driver Profile")
        .WithDescription("Returns driver's public profile (name and phone) for service-to-service calls.")
        .Produces<ApiResponse<DriverProfileResponse>>(StatusCodes.Status200OK)
        .Produces<ApiResponse<DriverProfileResponse>>(StatusCodes.Status404NotFound);

        return app;
    }
}
