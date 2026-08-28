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
                var userIdClaim = user.FindFirstValue(ClaimTypes.NameIdentifier);

                if (string.IsNullOrEmpty(userIdClaim) || !Guid.TryParse(userIdClaim, out var customerId))
                {
                    return Results.Unauthorized();
                }

                var command = new UpdateCartItemCommand(customerId, productId, request.Quantity);

                var result = await sender.Send(command, cancellationToken);

                if (result.IsSuccess)
                {
                    return Results.Ok(result.Value);
                }

                return Results.BadRequest(new
                {
                    Error = result.Error?.Code.ToString(),
                    Message = result.Error?.Message
                });
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
