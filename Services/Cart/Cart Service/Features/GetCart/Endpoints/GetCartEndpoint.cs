using Blocks.Contracts.Http;
using Blocks.Contracts.Security;
using Blocks.Domain.Errors;
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
            var handler = async (
                ISender sender,
                ClaimsPrincipal user,
                CancellationToken cancellationToken) =>
            {
                var customerIdClaim = user.FindFirstValue(FlowerClaimTypes.CustomerId)
                                      ?? user.FindFirstValue(ClaimTypes.NameIdentifier);

                if (string.IsNullOrEmpty(customerIdClaim) || !Guid.TryParse(customerIdClaim, out var customerId))
                {
                    return Results.Json(
                        ApiResponse<GetCartResponse>.Fail(Error.Unauthorized("You are not authorized to access this resource.")),
                        statusCode: StatusCodes.Status401Unauthorized);
                }

                var result = await sender.Send(new GetCartQuery(customerId), cancellationToken);

                if (result.IsSuccess)
                {
                    return Results.Ok(ApiResponse<GetCartResponse>.Ok(result.Value));
                }

                return Results.Json(
                    ApiResponse<GetCartResponse>.Fail(result.Error!),
                    statusCode: result.Error!.StatusCode == 0 ? StatusCodes.Status400BadRequest : result.Error.StatusCode);
            };

            app.MapGet("/cart", handler)
                .WithName("GetCartSummary")
                .WithTags("Cart")
                .WithSummary("Get Cart Summary (SCRUM-29 / SCRUM-107)")
                .WithDescription("Returns every line item in the authenticated user's server-side cart, with current price and stock flags re-checked at read time.")
                .Produces<ApiResponse<GetCartResponse>>(StatusCodes.Status200OK)
                .Produces<ApiResponse<GetCartResponse>>(StatusCodes.Status401Unauthorized)
                .RequireAuthorization();

            app.MapGet("/api/cart", handler)
                .WithName("GetCartApi")
                .WithTags("Cart")
                .WithSummary("Get Cart Summary (SCRUM-29 / SCRUM-107)")
                .Produces<ApiResponse<GetCartResponse>>(StatusCodes.Status200OK)
                .Produces<ApiResponse<GetCartResponse>>(StatusCodes.Status401Unauthorized)
                .RequireAuthorization();
        }
    }
}
