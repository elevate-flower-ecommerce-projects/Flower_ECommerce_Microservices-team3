using Blocks.Contracts.Http;
using Blocks.Contracts.Security;
using Blocks.Domain.Errors;
using Cart_Service.Features.Cart.ViewModels;
using Cart_Service.Features.UpdateCartItemQuantity.Commands;
using Cart_Service.Features.UpdateCartItemQuantity.DTOs;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace Cart_Service.Features.UpdateCartItemQuantity.Endpoints
{
    public static class UpdateCartItemEndpoint
    {
        public static void MapUpdateCartItemEndpoint(this IEndpointRouteBuilder app)
        {
            app.MapPatch("/api/cart/items/{productId:guid}", async (
                Guid productId,
                [FromBody] UpdateCartItemDto request,
                ISender sender,
                ClaimsPrincipal user,
                CancellationToken cancellationToken) =>
            {
                var customerIdClaim = user.FindFirstValue(FlowerClaimTypes.CustomerId)
                                      ?? user.FindFirstValue(ClaimTypes.NameIdentifier);

                if (string.IsNullOrEmpty(customerIdClaim) || !Guid.TryParse(customerIdClaim, out var customerId))
                {
                    return Results.Json(
                        ApiResponse<CartResponse>.Fail(Error.Unauthorized("You are not authorized to access this resource.")),
                        statusCode: StatusCodes.Status401Unauthorized);
                }

                var command = new UpdateCartItemCommand(customerId, productId, request.Quantity);

                var result = await sender.Send(command, cancellationToken);

                if (result.IsSuccess)
                {
                    return Results.Ok(ApiResponse<CartResponse>.Ok(result.Value));
                }

                return Results.Json(
                    ApiResponse<CartResponse>.Fail(result.Error!),
                    statusCode: result.Error!.StatusCode == 0 ? StatusCodes.Status400BadRequest : result.Error.StatusCode);
            })
            .WithName("UpdateCartItemQuantity")
            .WithTags("Cart")
            .WithSummary("Update quantity of an item in the cart")
            .WithDescription("Updates the item quantity for the authenticated customer. If quantity is 0, the item is removed.")
            .Produces(StatusCodes.Status200OK)
            .Produces(StatusCodes.Status400BadRequest)
            .Produces(StatusCodes.Status401Unauthorized)
            .RequireAuthorization();
        }
    }
}
