using System.Security.Claims;
using Blocks.Contracts.Http;
using Blocks.Contracts.Security;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Order___Fulfillment_Service.Entities.Enums;
using Order___Fulfillment_Service.Features.DriverFulfillment.AvailableOrders.DTOs;
using Order___Fulfillment_Service.Features.DriverFulfillment.Common;
using Order___Fulfillment_Service.Features.DriverFulfillment.OrderHistory.Queries;

namespace Order___Fulfillment_Service.Features.DriverFulfillment.OrderHistory;

public static class GetOrderHistoryEndpoint
{
    public static IEndpointRouteBuilder MapGetOrderHistoryEndpoint(this IEndpointRouteBuilder app)
    {
        app.MapGet("/drivers/me/orders", async (
            [FromQuery] int? page,
            [FromQuery] int? pageSize,
            [FromQuery] OrderStatus? status,
            ClaimsPrincipal user,
            IMediator mediator,
            CancellationToken ct) =>
        {
            var driverId = user.GetDriverId();
            if (driverId is null)
            {
                return Results.Json(
                    ApiResponse<AvailableOrderListDto>.Fail(DriverErrors.DriverUnauthorized()),
                    statusCode: 401);
            }

            var query = new GetOrderHistoryQuery(driverId.Value, page ?? 1, pageSize ?? 10, status);
            var result = await mediator.Send(query, ct);

            if (result.IsFailure)
            {
                return Results.Json(
                    ApiResponse<AvailableOrderListDto>.Fail(result.Error!),
                    statusCode: result.Error!.StatusCode);
            }

            return Results.Ok(ApiResponse<AvailableOrderListDto>.Ok(result.Value));
        })
        .WithName("GetDriverOrders")
        .WithTags("Driver Fulfillment")
        .RequireAuthorization(FlowerClaimTypes.DriverPolicy);

        return app;
    }
}
