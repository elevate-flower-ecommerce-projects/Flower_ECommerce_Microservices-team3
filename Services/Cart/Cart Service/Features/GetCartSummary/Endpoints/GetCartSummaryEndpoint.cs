using Blocks.Contracts.Http;
using Blocks.Contracts.Security;
using Blocks.Domain.Errors;
using Cart_Service.Features.Cart.DTOs;
using Cart_Service.Features.GetCartSummary.Queries;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using System.Globalization;
using System.Security.Claims;

namespace Cart_Service.Features.GetCartSummary.Endpoints;

// =========================================================================================================
// [TEMPORARY BUILD] Temporary placeholder endpoint for GET /cart (SCRUM-29 / SCRUM-107) until intern finishes.
// =========================================================================================================

public static class GetCartSummaryEndpoint
{
    public static void MapGetCartSummaryEndpoint(this IEndpointRouteBuilder app)
    {
        app.MapGet("/cart", async (
            [FromHeader(Name = "Accept-Language")] string? acceptLanguage,
            ISender sender,
            ClaimsPrincipal user,
            CancellationToken cancellationToken) =>
        {
            var customerIdClaim = user.FindFirstValue(FlowerClaimTypes.CustomerId)
                                  ?? user.FindFirstValue(ClaimTypes.NameIdentifier);

            if (string.IsNullOrEmpty(customerIdClaim) || !Guid.TryParse(customerIdClaim, out var customerId))
            {
                return Results.Json(
                    ApiResponse<CartSummaryDto>.Fail(Error.Unauthorized("You are not authorized to access this resource.")),
                    statusCode: StatusCodes.Status401Unauthorized);
            }

            var language = !string.IsNullOrWhiteSpace(acceptLanguage)
                ? (acceptLanguage.StartsWith("ar", StringComparison.OrdinalIgnoreCase) ? "ar" : "en")
                : (CultureInfo.CurrentCulture.TwoLetterISOLanguageName.StartsWith("ar", StringComparison.OrdinalIgnoreCase) ? "ar" : "en");

            var query = new GetCartSummaryQuery(customerId, language);
            var result = await sender.Send(query, cancellationToken);

            if (result.IsSuccess)
            {
                return Results.Ok(ApiResponse<CartSummaryDto>.Ok(result.Value));
            }

            return Results.Json(
                ApiResponse<CartSummaryDto>.Fail(result.Error!),
                statusCode: result.Error!.StatusCode);
        })
        .WithName("GetCartSummary")
        .WithTags("Cart")
        .WithSummary("Get Cart Summary (SCRUM-29 / SCRUM-107)")
        .WithDescription("Returns every line item in the authenticated user's server-side cart, with current price and stock flags, plus recalculated subtotal/total.")
        .Produces<ApiResponse<CartSummaryDto>>(StatusCodes.Status200OK)
        .Produces<ApiResponse<CartSummaryDto>>(StatusCodes.Status401Unauthorized)
        .Produces<ApiResponse<CartSummaryDto>>(StatusCodes.Status500InternalServerError)
        .RequireAuthorization();

        // Also map /api/cart for API route compatibility
        app.MapGet("/api/cart", async (
            [FromHeader(Name = "Accept-Language")] string? acceptLanguage,
            ISender sender,
            ClaimsPrincipal user,
            CancellationToken cancellationToken) =>
        {
            var customerIdClaim = user.FindFirstValue(FlowerClaimTypes.CustomerId)
                                  ?? user.FindFirstValue(ClaimTypes.NameIdentifier);

            if (string.IsNullOrEmpty(customerIdClaim) || !Guid.TryParse(customerIdClaim, out var customerId))
            {
                return Results.Json(
                    ApiResponse<CartSummaryDto>.Fail(Error.Unauthorized("You are not authorized to access this resource.")),
                    statusCode: StatusCodes.Status401Unauthorized);
            }

            var language = !string.IsNullOrWhiteSpace(acceptLanguage)
                ? (acceptLanguage.StartsWith("ar", StringComparison.OrdinalIgnoreCase) ? "ar" : "en")
                : (CultureInfo.CurrentCulture.TwoLetterISOLanguageName.StartsWith("ar", StringComparison.OrdinalIgnoreCase) ? "ar" : "en");

            var query = new GetCartSummaryQuery(customerId, language);
            var result = await sender.Send(query, cancellationToken);

            if (result.IsSuccess)
            {
                return Results.Ok(ApiResponse<CartSummaryDto>.Ok(result.Value));
            }

            return Results.Json(
                ApiResponse<CartSummaryDto>.Fail(result.Error!),
                statusCode: result.Error!.StatusCode);
        })
        .ExcludeFromDescription()
        .RequireAuthorization();
    }
}
