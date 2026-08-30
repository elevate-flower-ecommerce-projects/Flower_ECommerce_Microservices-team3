using Blocks.Contracts.Http;
using Blocks.Contracts.Security;
using Blocks.Domain.Errors;
using Cart_Service.Features.Cart.DTOs;
using Cart_Service.Features.RemoveCartItem.Commands;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using System.Globalization;
using System.Security.Claims;

namespace Cart_Service.Features.RemoveCartItem.Endpoints;

// =========================================================================================================
// [TEMPORARY BUILD] Temporary placeholder endpoint for DELETE /cart/items/{id} (SCRUM-25 / SCRUM-97) until intern finishes.
// =========================================================================================================

public static class RemoveCartItemEndpoint
{
    public static void MapRemoveCartItemEndpoint(this IEndpointRouteBuilder app)
    {
        app.MapDelete("/cart/items/{id:guid}", async (
            Guid id,
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

            var command = new RemoveCartItemCommand(customerId, id, language);
            var result = await sender.Send(command, cancellationToken);

            if (result.IsSuccess)
            {
                return Results.Ok(ApiResponse<CartSummaryDto>.Ok(result.Value, "Cart line removed successfully"));
            }

            return Results.Json(
                ApiResponse<CartSummaryDto>.Fail(result.Error!),
                statusCode: result.Error!.StatusCode);
        })
        .WithName("RemoveCartItem")
        .WithTags("Cart")
        .WithSummary("Remove Item from Cart (SCRUM-25 / SCRUM-97)")
        .WithDescription("Removes a single line from the authenticated user's cart and returns the updated cart summary.")
        .Produces<ApiResponse<CartSummaryDto>>(StatusCodes.Status200OK)
        .Produces<ApiResponse<CartSummaryDto>>(StatusCodes.Status401Unauthorized)
        .Produces<ApiResponse<CartSummaryDto>>(StatusCodes.Status404NotFound)
        .Produces<ApiResponse<CartSummaryDto>>(StatusCodes.Status500InternalServerError)
        .RequireAuthorization();

        // Also map /api/cart/items/{id} for compatibility
        app.MapDelete("/api/cart/items/{id:guid}", async (
            Guid id,
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

            var command = new RemoveCartItemCommand(customerId, id, language);
            var result = await sender.Send(command, cancellationToken);

            if (result.IsSuccess)
            {
                return Results.Ok(ApiResponse<CartSummaryDto>.Ok(result.Value, "Cart line removed successfully"));
            }

            return Results.Json(
                ApiResponse<CartSummaryDto>.Fail(result.Error!),
                statusCode: result.Error!.StatusCode);
        })
        .ExcludeFromDescription()
        .RequireAuthorization();
    }
}
