using System.Security.Claims;
using Blocks.Contracts.Http;
using Blocks.Contracts.Security;
using MediatR;
using Order___Fulfillment_Service.Features.DriverFulfillment.ActiveOrder.Queries;
using Order___Fulfillment_Service.Features.DriverFulfillment.Common;
using Order___Fulfillment_Service.Features.DriverFulfillment.Common.DTOs;

namespace Order___Fulfillment_Service.Features.DriverFulfillment.ActiveOrder;

public static class GetActiveOrderEndpoint
{
    public static IEndpointRouteBuilder MapGetActiveOrderEndpoint(this IEndpointRouteBuilder app)
    {
        app.MapGet("/drivers/me/active-order", async (
            ClaimsPrincipal user,
            IMediator mediator,
            CancellationToken ct) =>
        {
            var driverId = user.GetDriverId();
            if (driverId is null)
            {
                return Results.Json(
                    ApiResponse<DriverOrderDetailDto?>.Fail(DriverErrors.DriverUnauthorized()),
                    statusCode: 401);
            }

            var query = new GetActiveOrderQuery(driverId.Value);
            var result = await mediator.Send(query, ct);

            if (result.IsFailure)
            {
                return Results.Json(
                    ApiResponse<DriverOrderDetailDto?>.Fail(result.Error!),
                    statusCode: result.Error!.StatusCode);
            }

            return Results.Ok(ApiResponse<DriverOrderDetailDto?>.Ok(result.Value));
        })
        .WithName("GetActiveOrder")
        .WithTags("Driver Fulfillment")
        .RequireAuthorization(FlowerClaimTypes.DriverPolicy);

        return app;
    }
}
