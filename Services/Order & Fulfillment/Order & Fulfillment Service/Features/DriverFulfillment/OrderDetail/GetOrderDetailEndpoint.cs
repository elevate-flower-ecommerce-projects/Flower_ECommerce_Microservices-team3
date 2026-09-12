using System.Security.Claims;
using Blocks.Contracts.Http;
using Blocks.Contracts.Security;
using MediatR;
using Order___Fulfillment_Service.Features.DriverFulfillment.Common;
using Order___Fulfillment_Service.Features.DriverFulfillment.Common.DTOs;
using Order___Fulfillment_Service.Features.DriverFulfillment.OrderDetail.Queries;

namespace Order___Fulfillment_Service.Features.DriverFulfillment.OrderDetail;

public static class GetOrderDetailEndpoint
{
    public static IEndpointRouteBuilder MapGetOrderDetailEndpoint(this IEndpointRouteBuilder app)
    {
        app.MapGet("/drivers/me/orders/{orderId:guid}", async (
            Guid orderId,
            ClaimsPrincipal user,
            IMediator mediator,
            CancellationToken ct) =>
        {
            var driverId = user.GetDriverId();
            if (driverId is null)
            {
                return Results.Json(
                    ApiResponse<DriverOrderDetailDto>.Fail(DriverErrors.DriverUnauthorized()),
                    statusCode: 401);
            }

            var query = new GetOrderDetailQuery(driverId.Value, orderId);
            var result = await mediator.Send(query, ct);

            if (result.IsFailure)
            {
                return Results.Json(
                    ApiResponse<DriverOrderDetailDto>.Fail(result.Error!),
                    statusCode: result.Error!.StatusCode);
            }

            return Results.Ok(ApiResponse<DriverOrderDetailDto>.Ok(result.Value));
        })
        .WithName("GetDriverOrderDetail")
        .WithTags("Driver Fulfillment")
        .RequireAuthorization(FlowerClaimTypes.DriverPolicy);

        return app;
    }
}
