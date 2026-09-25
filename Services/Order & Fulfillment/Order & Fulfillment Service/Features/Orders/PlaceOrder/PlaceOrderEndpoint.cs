using System.Security.Claims;
using Blocks.Contracts.Http;
using Blocks.Contracts.Security;
using Blocks.Domain.Errors;
using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Order___Fulfillment_Service.Features.Orders.PlaceOrder.Commands;
using Order___Fulfillment_Service.Features.Orders.PlaceOrder.DTOs;

namespace Order___Fulfillment_Service.Features.Orders.PlaceOrder;

public static class PlaceOrderEndpoint
{
    public static IEndpointRouteBuilder MapPlaceOrderEndpoint(this IEndpointRouteBuilder app)
    {
        var handler = async (
            PlaceOrderRequest request,
            IMediator mediator,
            ClaimsPrincipal user,
            HttpContext httpContext,
            CancellationToken ct) =>
        {
            var customerIdClaim = user.FindFirstValue(FlowerClaimTypes.CustomerId)
                ?? user.FindFirstValue(ClaimTypes.NameIdentifier);

            if (string.IsNullOrEmpty(customerIdClaim) || !Guid.TryParse(customerIdClaim, out var customerId))
            {
                return Results.Json(
                    ApiResponse<PlaceOrderResponse>.Fail(Error.Forbidden("Access denied.")),
                    statusCode: StatusCodes.Status403Forbidden);
            }

            var authHeader = httpContext.Request.Headers.Authorization.ToString();
            var bearerToken = authHeader.StartsWith("Bearer ", StringComparison.OrdinalIgnoreCase)
                ? authHeader[7..].Trim()
                : authHeader;

            var email = user.FindFirstValue(ClaimTypes.Email) ?? "customer@example.com";
            var name = user.FindFirstValue(ClaimTypes.Name) ?? "Valued Customer";
            var phone = user.FindFirstValue(ClaimTypes.MobilePhone) ?? user.FindFirstValue("phone") ?? string.Empty;

            var command = new PlaceOrderCommand(
                CustomerId: customerId,
                CustomerEmail: email,
                CustomerName: name,
                CustomerPhone: phone,
                BearerToken: bearerToken,
                Request: request
            );

            var result = await mediator.Send(command, ct);
            if (result.IsFailure)
            {
                return Results.Json(
                    ApiResponse<PlaceOrderResponse>.Fail(result.Error!),
                    statusCode: result.Error!.StatusCode == 0 ? StatusCodes.Status400BadRequest : result.Error.StatusCode);
            }

            return Results.Ok(ApiResponse<PlaceOrderResponse>.Ok(result.Value, "Order placed successfully."));
        };

        app.MapPost("/orders", handler)
            .WithName("PlaceOrder")
            .WithTags("Orders")
            .WithSummary("Place a new order from current cart")
            .WithDescription("Converts authenticated customer's cart into an order, calculates delivery fees from nearest covering store, initiates payment (COD or Paymob Card), and clears cart upon confirmation.")
            .Produces<ApiResponse<PlaceOrderResponse>>(StatusCodes.Status200OK)
            .Produces<ApiResponse<PlaceOrderResponse>>(StatusCodes.Status400BadRequest)
            .RequireAuthorization();

        app.MapPost("/api/orders", handler).ExcludeFromDescription().RequireAuthorization();
        app.MapPost("/api/v1/orders", handler).ExcludeFromDescription().RequireAuthorization();

        return app;
    }
}
