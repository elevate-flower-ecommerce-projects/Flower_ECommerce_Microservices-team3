using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Microsoft.EntityFrameworkCore;
using Order___Fulfillment_Service.Entities.Enums;
using Order___Fulfillment_Service.Persistence;
using Order___Fulfillment_Service.Services;

namespace Order___Fulfillment_Service.Features.Orders.MarkPaid;

public static class MarkPaidEndpoint
{
    public static IEndpointRouteBuilder MapMarkPaidEndpoint(this IEndpointRouteBuilder app)
    {
        app.MapPost("/internal/orders/{orderId:guid}/mark-paid", async (
            Guid orderId,
            FlowersOrderDbContext dbContext,
            ICartServiceClient cartService,
            IUnitOfWork unitOfWork,
            CancellationToken ct) =>
        {
            var order = await dbContext.Orders
                .FirstOrDefaultAsync(o => o.Id == orderId, ct);

            if (order is null)
            {
                return Results.NotFound(new { message = "Order not found" });
            }

            order.Status = OrderStatus.Preparing;
            order.UpdatedAt = DateTime.UtcNow;

            await unitOfWork.SaveChangesAsync(ct);

            if (order.CartId.HasValue)
            {
                await cartService.ClearCartAsync(order.CartId.Value, ct: ct);
            }

            return Results.Ok(new { message = "Order marked as paid and moved to Preparing status successfully." });
        })
        .WithName("MarkOrderPaidInternal")
        .WithTags("Internal");

        return app;
    }
}
