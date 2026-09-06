using Blocks.Contracts.Http;
using Blocks.Contracts.Security;
using Blocks.Domain.Errors;
using MediatR;
using System.Security.Claims;

namespace Order___Fulfillment_Service.Features.Checkout.GetCheckoutDetails;

public static class GetCheckoutDetailsEndpoint
{
    public static IEndpointRouteBuilder MapGetCheckoutDetailsEndpoint(this IEndpointRouteBuilder app)
    {
        var handler = async (
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
                    ApiResponse<CheckoutDetailsResponse>.Fail(Error.Unauthorized("You are not authorized to access this resource.")),
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

            var result = await sender.Send(new GetCheckoutDetailsQuery(customerId, bearerToken), cancellationToken);

            if (result.IsSuccess)
            {
                return Results.Ok(ApiResponse<CheckoutDetailsResponse>.Ok(result.Value));
            }

            return Results.Json(
                ApiResponse<CheckoutDetailsResponse>.Fail(result.Error!),
                statusCode: result.Error!.StatusCode == 0 ? StatusCodes.Status400BadRequest : result.Error.StatusCode);
        };

        app.MapGet("/checkout/details", handler)
            .WithName("GetCheckoutDetails")
            .WithTags("Checkout")
            .WithSummary("Get Checkout Details")
            .WithDescription("Returns checkout details including cartId, addressId, isServiceable, subtotal, delivery fee, grand total, ETA, and payment methods.")
            .Produces<ApiResponse<CheckoutDetailsResponse>>(StatusCodes.Status200OK)
            .Produces<ApiResponse<CheckoutDetailsResponse>>(StatusCodes.Status401Unauthorized)
            .Produces<ApiResponse<CheckoutDetailsResponse>>(StatusCodes.Status422UnprocessableEntity)
            .RequireAuthorization();

        return app;
    }
}
