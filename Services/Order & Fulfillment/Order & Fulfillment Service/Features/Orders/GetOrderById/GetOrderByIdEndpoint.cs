using Blocks.Contracts.Http;
using Blocks.Contracts.Security;
using Blocks.Domain.Errors;
using MediatR;
using Order___Fulfillment_Service.Features.Orders.GetOrderById.DTOs;
using Order___Fulfillment_Service.Features.Orders.GetOrderById.Queries;
using System.Security.Claims;

namespace Order___Fulfillment_Service.Features.Orders.GetOrderById
{
    public static class GetOrderByIdEndpoint
    {
        public static IEndpointRouteBuilder MapGetOrderByIdEndpoint(this IEndpointRouteBuilder app)
        {
            app.MapGet("/orders/{orderId:guid}", async (
                Guid orderId,
                IMediator mediator,
                ClaimsPrincipal user,
                CancellationToken ct) =>
            {
              
                var customerIdClaim = user.FindFirstValue(FlowerClaimTypes.CustomerId);
                if (string.IsNullOrEmpty(customerIdClaim) || !Guid.TryParse(customerIdClaim, out var customerId))
                {
                    return Results.Json(
                        ApiResponse<OrderDetailDto>.Fail(
                            Error.Forbidden("Access denied.")),
                        statusCode: 403);
                }
               
                var result = await mediator.Send(
                    new GetOrderByIdQuery(customerId, orderId), ct);
              
                if (result.IsFailure)
                {
                    return Results.Json(
                        ApiResponse<OrderDetailDto>.Fail(result.Error!),
                        statusCode: result.Error!.StatusCode);
                }
               
                return Results.Ok(
                    ApiResponse<OrderDetailDto>.Ok(result.Value));
            })
            .WithName("GetOrderById")
            .WithTags("Orders")
            .RequireAuthorization();
            return app;
        }
    }

}
