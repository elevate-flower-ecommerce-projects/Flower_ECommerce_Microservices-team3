using Blocks.Contracts.Http;
using Blocks.Contracts.Pagination;
using Blocks.Contracts.Security;
using Blocks.Domain.Errors;
using MediatR;
using Order___Fulfillment_Service.Entities.Enums;
using Order___Fulfillment_Service.Features.Orders.GetOrders.DTOs;
using Order___Fulfillment_Service.Features.Orders.GetOrders.Queries;
using System.Security.Claims;

namespace Order___Fulfillment_Service.Features.Orders.GetOrders
{
    public static class GetOrdersEndpoint
    {
        public static IEndpointRouteBuilder MapGetOrdersEndpoint(this IEndpointRouteBuilder app)
        {
            app.MapGet("/orders", async (
                IMediator mediator,
                ClaimsPrincipal user,
                int page = 1,
                int pageSize = 10,
                OrderStatus? status = null,
                CancellationToken ct = default) =>
            {
                
                var customerIdClaim = user.FindFirstValue(FlowerClaimTypes.CustomerId);
                if (string.IsNullOrEmpty(customerIdClaim) || !Guid.TryParse(customerIdClaim, out var customerId))
                {
                    return Results.Json(
                        ApiResponse<PagedResult<OrderListItemDto>>.Fail(
                            Error.Forbidden("Access denied.")),
                        statusCode: 403);
                }
                
                var result = await mediator.Send(
                    new GetOrdersQuery(customerId, page, pageSize, status), ct);
               
                if (result.IsFailure)
                {
                    return Results.Json(
                        ApiResponse<PagedResult<OrderListItemDto>>.Fail(result.Error!),
                        statusCode: result.Error!.StatusCode);
                }
               
                return Results.Ok(
                    ApiResponse<PagedResult<OrderListItemDto>>.Ok(result.Value));
            })
            .WithName("GetOrders")
            .WithTags("Orders")
            .RequireAuthorization();
            return app;
        }
    }
}
