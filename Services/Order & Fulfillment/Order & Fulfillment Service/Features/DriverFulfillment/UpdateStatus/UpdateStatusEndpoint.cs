using System.Security.Claims;
using Blocks.Contracts.Http;
using Blocks.Contracts.Security;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Order___Fulfillment_Service.Features.DriverFulfillment.Common;
using Order___Fulfillment_Service.Features.DriverFulfillment.UpdateStatus.Commands;
using Order___Fulfillment_Service.Features.DriverFulfillment.UpdateStatus.DTOs;

namespace Order___Fulfillment_Service.Features.DriverFulfillment.UpdateStatus;

public static class UpdateStatusEndpoint
{
    public static IEndpointRouteBuilder MapUpdateStatusEndpoint(this IEndpointRouteBuilder app)
    {
        app.MapPatch("/orders/{orderId:guid}/status", async (
            Guid orderId,
            [FromBody] UpdateOrderStatusRequestDto request,
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

            var command = new UpdateOrderStatusCommand(driverId.Value, orderId, request.NewStatus);
            var result = await mediator.Send(command, ct);

            if (result.IsFailure)
            {
                return Results.Json(
                    ApiResponse<object>.Fail(result.Error!),
                    statusCode: result.Error!.StatusCode);
            }

            return Results.Ok(ApiResponse<object?>.Ok(null, "Order status updated successfully."));
        })
        .WithName("UpdateOrderStatus")
        .WithTags("Driver Fulfillment")
        .RequireAuthorization(FlowerClaimTypes.DriverPolicy);

        return app;
    }
}
