using System.Globalization;
using System.Security.Claims;
using Blocks.Contracts.Common;
using Blocks.Contracts.Http;
using Blocks.Contracts.Security;
using Blocks.Domain.Errors;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Cart_Service.Features.AddToCartItem;

public static class AddToCartItemEndpoint
{
    public static void MapAddToCartItemEndpoint(this IEndpointRouteBuilder app)
    {
        var handler = async (
            [FromBody] AddToCartItemRequest request,
            [FromHeader(Name = "Accept-Language")] string? acceptLanguage,
            ISender sender,
            ClaimsPrincipal user,
            CancellationToken cancellationToken) =>
        {
            var customerIdClaim =
                user.FindFirstValue(FlowerClaimTypes.CustomerId)
                ?? user.FindFirstValue(ClaimTypes.NameIdentifier);

            if (!Guid.TryParse(customerIdClaim, out var customerId))
            {
                return Results.Json(
                    ApiResponse<CartSummaryResponse>.Fail(Error.Unauthorized("You are not authorized to access this resource.")),
                    statusCode: StatusCodes.Status401Unauthorized);
            }

            var language = acceptLanguage?
                                         .Split(',')[0]
                                         .Trim()
                       ?? CultureInfo.CurrentCulture.TwoLetterISOLanguageName;

            var command = new AddToCartItemCommand(
                customerId,
                request.ProductId,
                request.Quantity,
                language);

            var result = await sender.Send(
                command,
                cancellationToken);

            if (result.IsSuccess)
            {
                return Results.Ok(ApiResponse<CartSummaryResponse>.Ok(result.Value, "Item added to cart successfully"));
            }

            return Results.Json(
                ApiResponse<CartSummaryResponse>.Fail(result.Error!),
                statusCode: result.Error!.StatusCode == 0 ? StatusCodes.Status400BadRequest : result.Error.StatusCode);
        };

        app.MapPost("/api/v1/cart/items", handler)
            .WithName("AddToCartItem")
            .WithTags("Cart")
            .WithSummary("Add item to cart")
            .WithDescription(
                "Adds a product to the authenticated customer's cart " +
                "or increments its quantity if it already exists.")
            .Produces<ApiResponse<CartSummaryResponse>>(StatusCodes.Status200OK)
            .Produces<ApiResponse<CartSummaryResponse>>(StatusCodes.Status400BadRequest)
            .Produces<ApiResponse<CartSummaryResponse>>(StatusCodes.Status401Unauthorized)
            .Produces<ApiResponse<CartSummaryResponse>>(StatusCodes.Status404NotFound)
            .RequireAuthorization();

        app.MapPost("/cart/items", handler).ExcludeFromDescription().RequireAuthorization();
        app.MapPost("/api/cart/items", handler).ExcludeFromDescription().RequireAuthorization();
        app.MapPost("/cart/cart/items", handler).ExcludeFromDescription().RequireAuthorization();
        app.MapPost("/items", handler).ExcludeFromDescription().RequireAuthorization();
    }
}