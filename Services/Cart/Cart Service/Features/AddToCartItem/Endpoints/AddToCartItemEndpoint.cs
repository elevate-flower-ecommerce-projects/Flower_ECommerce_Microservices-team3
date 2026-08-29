using Blocks.Contracts.Http;
using Blocks.Contracts.Security;
using Blocks.Domain.Errors;
using Cart_Service.Features.AddToCartItem.Commands;
using Cart_Service.Features.AddToCartItem.DTOs;
using Cart_Service.Features.Cart.DTOs;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using System.Globalization;
using System.Security.Claims;

namespace Cart_Service.Features.AddToCartItem.Endpoints;

// =========================================================================================================
// [TEMPORARY BUILD] Temporary placeholder endpoint for POST /cart/items (SCRUM-11 / SCRUM-87 / SCRUM-88) until intern finishes.
// =========================================================================================================

public static class AddToCartItemEndpoint
{
    public static void MapAddToCartItemEndpoint(this IEndpointRouteBuilder app)
    {
        app.MapPost("/cart/items", async (
            [FromBody] AddToCartItemRequestDto request,
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

            var command = new AddToCartItemCommand(customerId, request.ProductId, request.Quantity, language);
            var result = await sender.Send(command, cancellationToken);

            if (result.IsSuccess)
            {
                return Results.Json(
                    ApiResponse<CartSummaryDto>.Ok(result.Value, "Item added (or existing line incremented) successfully"),
                    statusCode: StatusCodes.Status201Created);
            }

            return Results.Json(
                ApiResponse<CartSummaryDto>.Fail(result.Error!),
                statusCode: result.Error!.StatusCode);
        })
        .WithName("AddToCartItem")
        .WithTags("Cart")
        .WithSummary("Add Item to Cart (SCRUM-11 / SCRUM-87 / SCRUM-88)")
        .WithDescription("Adds a product to the authenticated user's cart, or increments quantity if it already exists.")
        .Produces<ApiResponse<CartSummaryDto>>(StatusCodes.Status201Created)
        .Produces<ApiResponse<CartSummaryDto>>(StatusCodes.Status400BadRequest)
        .Produces<ApiResponse<CartSummaryDto>>(StatusCodes.Status401Unauthorized)
        .Produces<ApiResponse<CartSummaryDto>>(StatusCodes.Status404NotFound)
        .Produces<ApiResponse<CartSummaryDto>>(StatusCodes.Status500InternalServerError)
        .RequireAuthorization();

        // Also map /api/cart/items for route compatibility
        app.MapPost("/api/cart/items", async (
            [FromBody] AddToCartItemRequestDto request,
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

            var command = new AddToCartItemCommand(customerId, request.ProductId, request.Quantity, language);
            var result = await sender.Send(command, cancellationToken);

            if (result.IsSuccess)
            {
                return Results.Json(
                    ApiResponse<CartSummaryDto>.Ok(result.Value, "Item added (or existing line incremented) successfully"),
                    statusCode: StatusCodes.Status201Created);
            }

            return Results.Json(
                ApiResponse<CartSummaryDto>.Fail(result.Error!),
                statusCode: result.Error!.StatusCode);
        })
        .ExcludeFromDescription()
        .RequireAuthorization();
    }
}
