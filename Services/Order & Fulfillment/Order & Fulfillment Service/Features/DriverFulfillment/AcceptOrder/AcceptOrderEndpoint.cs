using System.Security.Claims;
using Blocks.Contracts.Http;
using Blocks.Contracts.Security;
using MediatR;
using Order___Fulfillment_Service.Features.DriverFulfillment.AcceptOrder.Commands;
using Order___Fulfillment_Service.Features.DriverFulfillment.Common;

namespace Order___Fulfillment_Service.Features.DriverFulfillment.AcceptOrder;

public static class AcceptOrderEndpoint
{
    public static IEndpointRouteBuilder MapAcceptOrderEndpoint(this IEndpointRouteBuilder app)
    {
        app.MapPost("/drivers/me/orders/{orderId:guid}/accept", async (
            Guid orderId,
            ClaimsPrincipal user,
            IMediator mediator,
            CancellationToken ct) =>
        {
            var driverId = user.GetDriverId();
            if (driverId is null)
            {
                return Results.Json(
                    ApiResponse<object>.Fail(DriverErrors.DriverUnauthorized()),
                    statusCode: 401);
            }

            var command = new AcceptOrderCommand(driverId.Value, orderId);
            var result = await mediator.Send(command, ct);

            if (result.IsFailure)
            {
                return Results.Json(
                    ApiResponse<object>.Fail(result.Error!),
                    statusCode: result.Error!.StatusCode);
            }

            return Results.Ok(ApiResponse<object?>.Ok(null, "Order claimed successfully."));
        })
        .WithName("AcceptOrder")
        .WithTags("Driver Fulfillment")
        .RequireAuthorization(FlowerClaimTypes.DriverPolicy);

        return app;
    }
}
