using Blocks.Contracts.Http;
using Cart_Service.Features.GetCart.Queries;
using Cart_Service.Features.GetCart.ViewModels;
using MediatR;
using System.Security.Claims;

namespace Cart_Service.Features.GetCart.Endpoints
{
    public static class GetCartEndpoint
    {
        public static void MapGetCartEndpoint(this IEndpointRouteBuilder app)
        {
            app.MapGet("/api/cart", async (
                ISender sender,
                ClaimsPrincipal user,
                CancellationToken cancellationToken) =>
            {
                var userIdClaim = user.FindFirstValue(ClaimTypes.NameIdentifier);

                if (string.IsNullOrEmpty(userIdClaim) || !Guid.TryParse(userIdClaim, out var customerId))
                {
                    return Results.Unauthorized();
                }

                var result = await sender.Send(new GetCartQuery(customerId), cancellationToken);

                if (result.IsSuccess)
                {
                    return Results.Ok(ApiResponse<GetCartResponse>.Ok(result.Value));
                }

                return Results.Json(
                    ApiResponse<GetCartResponse>.Fail(result.Error!),
                    statusCode: result.Error!.StatusCode);
            })
            .WithName("GetCart")
            .WithTags("Cart")
            .Produces<ApiResponse<GetCartResponse>>(StatusCodes.Status200OK)
            .Produces<ApiResponse<GetCartResponse>>(StatusCodes.Status401Unauthorized)
            .RequireAuthorization();
        }
    }
}
