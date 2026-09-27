using System.Security.Claims;
using Blocks.Contracts.Http;
using Blocks.Contracts.Security;
using Cart_Service.Persistence;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using Microsoft.EntityFrameworkCore;

namespace Cart_Service.Features.ClearCart;

public static class ClearCartEndpoint
{
    public static void MapClearCartEndpoint(this IEndpointRouteBuilder app)
    {
        var handler = async (
            [FromQuery] Guid? cartId,
            FlowersCartDbContext db,
            ClaimsPrincipal user,
            CancellationToken ct) =>
        {
            Guid? customerId = null;
            var customerIdClaim = user.FindFirstValue(FlowerClaimTypes.CustomerId)
                                  ?? user.FindFirstValue(ClaimTypes.NameIdentifier);
            if (!string.IsNullOrEmpty(customerIdClaim) && Guid.TryParse(customerIdClaim, out var parsedCustomerId))
            {
                customerId = parsedCustomerId;
            }

            Entities.Cart? cart = null;

            if (cartId.HasValue && cartId.Value != Guid.Empty)
            {
                cart = await db.Carts
                    .Include(c => c.Items)
                    .FirstOrDefaultAsync(c => c.Id == cartId.Value, ct);
            }
            else if (customerId.HasValue)
            {
                cart = await db.Carts
                    .Include(c => c.Items)
                    .FirstOrDefaultAsync(c => c.CustomerId == customerId.Value, ct);
            }

            if (cart is null)
            {
                return Results.Ok(ApiResponse<string>.Ok("Cart is already empty."));
            }

            cart.ClearItems();
            await db.SaveChangesAsync(ct);

            return Results.Ok(ApiResponse<string>.Ok("Cart cleared successfully."));
        };

        app.MapDelete("/cart", handler)
            .WithName("ClearCart")
            .WithTags("Cart")
            .WithSummary("Clear all items from Cart")
            .Produces<ApiResponse<string>>(StatusCodes.Status200OK);

        app.MapDelete("/api/cart", handler).ExcludeFromDescription();
        app.MapDelete("/api/cart/clear", handler).ExcludeFromDescription();
        app.MapPost("/api/cart/clear", handler).ExcludeFromDescription();
    }
}
