using Blocks.Contracts.Http;
using Blocks.Contracts.Security;
using Blocks.Domain.Errors;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace Order___Fulfillment_Service.Features.Checkout.EstimateDelivery;

public static class EstimateDeliveryEndpoint
{
    public static IEndpointRouteBuilder MapEstimateDeliveryEndpoint(this IEndpointRouteBuilder app)
    {
        var handler = async (
            [FromQuery] Guid addressId,
            ISender sender,
            ClaimsPrincipal user,
            HttpContext httpContext,
            CancellationToken cancellationToken) =>
        {
            var customerIdClaim = user.FindFirstValue(FlowerClaimTypes.CustomerId)
                                  ?? user.FindFirstValue(ClaimTypes.NameIdentifier);

            if (string.IsNullOrEmpty(customerIdClaim) || !Guid.TryParse(customerIdClaim, out var customerId))
            {
                return Results.Json(
                    ApiResponse<EstimateDeliveryResponse>.Fail(Error.Unauthorized("You are not authorized to access this resource.")),
                    statusCode: StatusCodes.Status401Unauthorized);
            }

            string? bearerToken = null;
            if (httpContext.Request.Headers.TryGetValue("Authorization", out var authHeader))
            {
                var headerStr = authHeader.ToString();
                if (headerStr.StartsWith("Bearer ", StringComparison.OrdinalIgnoreCase))
                {
                    bearerToken = headerStr["Bearer ".Length..].Trim();
                }
                else
                {
                    bearerToken = headerStr.Trim();
                }
            }

            var result = await sender.Send(new EstimateDeliveryQuery(customerId, addressId, bearerToken), cancellationToken);

            if (result.IsSuccess)
            {
                return Results.Ok(ApiResponse<EstimateDeliveryResponse>.Ok(result.Value));
            }

            return Results.Json(
                ApiResponse<EstimateDeliveryResponse>.Fail(result.Error!),
                statusCode: result.Error!.StatusCode == 0 ? StatusCodes.Status400BadRequest : result.Error.StatusCode);
        };

        app.MapGet("/checkout/estimate-delivery", handler)
            .WithName("EstimateDelivery")
            .WithTags("Checkout")
            .WithSummary("Estimate Delivery Time")
            .WithDescription("Calculates estimated delivery time for a specific address. Returns 422 if the address is outside store coverage.")
            .Produces<ApiResponse<EstimateDeliveryResponse>>(StatusCodes.Status200OK)
            .Produces<ApiResponse<EstimateDeliveryResponse>>(StatusCodes.Status401Unauthorized)
            .Produces<ApiResponse<EstimateDeliveryResponse>>(StatusCodes.Status404NotFound)
            .Produces<ApiResponse<EstimateDeliveryResponse>>(StatusCodes.Status422UnprocessableEntity)
            .RequireAuthorization();

        return app;
    }
}
