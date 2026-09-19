using Blocks.Contracts.Http;
using Blocks.Contracts.Security;
using Blocks.Domain.Errors;
using MediatR;
using Order___Fulfillment_Service.Features.Orders.ConfirmDelivery.Commands;
using System.Security.Claims;

namespace Order___Fulfillment_Service.Features.Orders.ConfirmDelivery
{
    public static class ConfirmDeliveryEndpoint
    {
        public static IEndpointRouteBuilder MapConfirmDeliveryEndpoint(this IEndpointRouteBuilder app)
        {
            app.MapPost("/orders/{orderId:guid}/confirm-delivery", async (
                    Guid orderId,
                    ClaimsPrincipal user,
                    IMediator mediator,
                    CancellationToken cancellationToken) =>
            {
               
                var customerIdClaim = user.FindFirstValue(FlowerClaimTypes.CustomerId)
                                     ?? user.FindFirstValue(ClaimTypes.NameIdentifier);
                if (string.IsNullOrEmpty(customerIdClaim) || !Guid.TryParse(customerIdClaim, out var customerId))
                {
                    return Results.Json(
                        ApiResponse<string>.Fail(
                            Error.Unauthorized("Invalid or missing user identity.")),
                        statusCode: StatusCodes.Status401Unauthorized);
                }
                var command = new ConfirmOrderDeliveryCommand(orderId, customerId);
                var result = await mediator.Send(command, cancellationToken);
                
                if (result.IsFailure)
                {
                    return Results.Json(
                        ApiResponse<string>.Fail(result.Error!),
                        statusCode: result.Error!.StatusCode);
                }
                return Results.Ok(ApiResponse<string>.Ok(result.Value!));
            })
                .WithName("ConfirmOrderDelivery")
                .WithTags("Orders")
                .Produces<ApiResponse<string>>(StatusCodes.Status200OK)
                .Produces<ApiResponse<string>>(StatusCodes.Status400BadRequest)
                .Produces<ApiResponse<string>>(StatusCodes.Status401Unauthorized)
                .Produces<ApiResponse<string>>(StatusCodes.Status403Forbidden)
                .Produces<ApiResponse<string>>(StatusCodes.Status404NotFound)
                .RequireAuthorization();
            return app;
        }
    }

}
