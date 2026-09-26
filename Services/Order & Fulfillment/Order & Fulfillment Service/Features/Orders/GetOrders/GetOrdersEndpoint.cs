using Blocks.Contracts.Http;
using Blocks.Contracts.Security;
using Blocks.Domain.Errors;
using MediatR;
using Order___Fulfillment_Service.Entities.Enums;
using Order___Fulfillment_Service.Features.Orders.GetOrders.DTOs;
using Order___Fulfillment_Service.Features.Orders.GetOrders.Queries;
using System.Globalization;
using System.Security.Claims;

namespace Order___Fulfillment_Service.Features.Orders.GetOrders
{
    public static class GetOrdersEndpoint
    {
        public static IEndpointRouteBuilder MapGetOrdersEndpoint(this IEndpointRouteBuilder app)
        {
            var handler = async (
                IMediator mediator,
                ClaimsPrincipal user,
                int page = 1,
                int pageSize = 10,
                OrderStatus? status = null,
                CancellationToken ct = default) =>
            {
                var isArabic = CultureInfo.CurrentUICulture.Name.StartsWith("ar");
                var customerIdClaim = user.FindFirstValue(FlowerClaimTypes.CustomerId)
                                      ?? user.FindFirstValue(ClaimTypes.NameIdentifier);

                if (string.IsNullOrEmpty(customerIdClaim) || !Guid.TryParse(customerIdClaim, out var customerId))
                {
                    var msg = "Access denied.";
                    var msgAr = isArabic ? "تم رفض الوصول." : msg;
                    return Results.Json(
                        FloweryApiResponse<OrderListData>.Failure(msg, "Forbidden", msgAr),
                        statusCode: StatusCodes.Status403Forbidden);
                }

                var result = await mediator.Send(
                    new GetOrdersQuery(customerId, page, pageSize, status), ct);

                if (result.IsFailure)
                {
                    var errorMsg = result.Error?.Message ?? "Failed to retrieve orders.";
                    var errorMsgAr = isArabic ? "فشل في جلب قائمة الطلبات." : errorMsg;
                    return Results.Json(
                        FloweryApiResponse<OrderListData>.Failure(errorMsg, "BadRequest", errorMsgAr),
                        statusCode: result.Error!.StatusCode == 0 ? StatusCodes.Status400BadRequest : result.Error.StatusCode);
                }

                var paged = result.Value;
                var orderListData = new OrderListData(
                    paged.Items,
                    new PaginationDto(
                        Page: paged.PageNumber,
                        PageSize: paged.PageSize,
                        TotalCount: paged.TotalCount,
                        TotalPages: paged.TotalPages,
                        HasNextPage: paged.HasNextPage,
                        HasPreviousPage: paged.HasPreviousPage
                    )
                );

                var successMsg = "Orders retrieved successfully.";
                var successMsgAr = isArabic ? "تم جلب الطلبات بنجاح." : successMsg;

                return Results.Ok(
                    FloweryApiResponse<OrderListData>.Success(orderListData, successMsg, successMsgAr));
            };

            app.MapGet("/orders", handler)
                .WithName("GetOrders")
                .WithTags("Orders")
                .WithSummary("List my orders")
                .WithDescription("Paged, newest-first list of the current user's orders.")
                .Produces<FloweryApiResponse<OrderListData>>(StatusCodes.Status200OK)
                .Produces<FloweryApiResponse<OrderListData>>(StatusCodes.Status401Unauthorized)
                .RequireAuthorization();

            app.MapGet("/api/orders", handler).ExcludeFromDescription().RequireAuthorization();
            app.MapGet("/api/v1/orders", handler).ExcludeFromDescription().RequireAuthorization();

            return app;
        }
    }
}
