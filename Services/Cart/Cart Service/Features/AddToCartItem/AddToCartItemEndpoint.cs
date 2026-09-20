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
        app.MapPost("/api/v1/cart/items",
            async (
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
                    return Result.Failure<CartSummaryResponse>(
                        Error.Unauthorized(
                            "You are not authorized to access this resource."));
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

                return result;
            })
            .WithName("AddToCartItem")
            .WithTags("Cart")
            .WithSummary("Add item to cart")
            .WithDescription(
                "Adds a product to the authenticated customer's cart " +
                "or increments its quantity if it already exists.")
            .RequireAuthorization();
    }
}