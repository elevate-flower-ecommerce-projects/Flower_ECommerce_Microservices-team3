using System.Security.Claims;
using Blocks.Contracts.Http;
using Blocks.Contracts.Security;
using Blocks.Domain.Errors;
using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;

namespace Order___Fulfillment_Service.Features.Orders.CancelOrder;

public static class CancelOrderEndpoint
{
    public static IEndpointRouteBuilder MapCancelOrderEndpoint(this IEndpointRouteBuilder app)
    {
        var handler = async (
            Guid orderId,
            CancelOrderRequest? request,
            IMediator mediator,
            ClaimsPrincipal user,
            CancellationToken ct) =>
        {
            var customerIdClaim = user.FindFirstValue(FlowerClaimTypes.CustomerId)
                ?? user.FindFirstValue(ClaimTypes.NameIdentifier);

            if (string.IsNullOrEmpty(customerIdClaim) || !Guid.TryParse(customerIdClaim, out var customerId))
            {
                return Results.Json(
                    ApiResponse<string>.Fail(Error.Forbidden("Access denied.")),
                    statusCode: StatusCodes.Status403Forbidden);
            }

            var command = new CancelOrderCommand(orderId, customerId, request?.Reason);
            var result = await mediator.Send(command, ct);

            if (result.IsFailure)
            {
                return Results.Json(
                    ApiResponse<string>.Fail(result.Error!),
                    statusCode: result.Error!.StatusCode == 0 ? StatusCodes.Status400BadRequest : result.Error.StatusCode);
            }

            return Results.Ok(ApiResponse<string>.Ok(result.Value, result.Value));
        };

        app.MapPost("/orders/{orderId:guid}/cancel", handler)
            .WithName("CancelOrder")
            .WithTags("Orders")
            .WithSummary("Cancel an order before driver pickup")
            .Produces<ApiResponse<string>>(StatusCodes.Status200OK)
            .Produces<ApiResponse<string>>(StatusCodes.Status400BadRequest)
            .RequireAuthorization();

        app.MapPost("/api/orders/{orderId:guid}/cancel", handler).ExcludeFromDescription().RequireAuthorization();

        return app;
    }
}
