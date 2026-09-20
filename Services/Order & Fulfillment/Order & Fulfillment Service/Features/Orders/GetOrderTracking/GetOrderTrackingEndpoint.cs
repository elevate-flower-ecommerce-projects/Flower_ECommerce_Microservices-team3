using Blocks.Contracts.Http;
using Blocks.Contracts.Security;
using Blocks.Domain.Errors;
using MediatR;
using Order___Fulfillment_Service.Features.Orders.GetOrderTracking.DTOs;
using System.Security.Claims;

namespace Order___Fulfillment_Service.Features.Orders.GetOrderTracking;

public static class GetOrderTrackingEndpoint
{
    public static IEndpointRouteBuilder MapGetOrderTrackingEndpoint(this IEndpointRouteBuilder app)
    {
        var handler = async (
            Guid orderId,
            IMediator mediator,
            ClaimsPrincipal user,
            CancellationToken cancellationToken) =>
        {
            var customerIdClaim = user.FindFirstValue(FlowerClaimTypes.CustomerId)
                                  ?? user.FindFirstValue(ClaimTypes.NameIdentifier);

            if (string.IsNullOrEmpty(customerIdClaim) || !Guid.TryParse(customerIdClaim, out var customerId))
            {
                return Results.Json(
                    ApiResponse<OrderTrackingDataDto>.Fail(
                        Error.Unauthorized("You are not authorized to access this resource.")),
                    statusCode: StatusCodes.Status401Unauthorized);
            }

            var result = await mediator.Send(
                new GetOrderTrackingOrchestrator(customerId, orderId), cancellationToken);

            if (result.IsSuccess)
            {
                return Results.Ok(ApiResponse<OrderTrackingDataDto>.Ok(result.Value));
            }

            return Results.Json(
                ApiResponse<OrderTrackingDataDto>.Fail(result.Error!),
                statusCode: result.Error!.StatusCode == 0
                    ? StatusCodes.Status400BadRequest
                    : result.Error.StatusCode);
        };

        app.MapGet("/orders/{orderId:guid}/tracking", handler)
            .WithName("GetOrderTracking")
            .WithTags("Tracking")
            .WithSummary("Get Order Tracking")
            .WithDescription("Returns live tracking for a customer's own order.")
            .Produces<ApiResponse<OrderTrackingDataDto>>(StatusCodes.Status200OK)
            .Produces<ApiResponse<OrderTrackingDataDto>>(StatusCodes.Status401Unauthorized)
            .Produces<ApiResponse<OrderTrackingDataDto>>(StatusCodes.Status404NotFound)
            .RequireAuthorization();

        return app;
    }
}
