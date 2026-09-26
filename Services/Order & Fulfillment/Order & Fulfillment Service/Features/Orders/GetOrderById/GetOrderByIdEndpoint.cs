using Blocks.Contracts.Http;
using Blocks.Contracts.Security;
using Blocks.Domain.Errors;
using MediatR;
using Order___Fulfillment_Service.Features.Orders.GetOrderById.DTOs;
using Order___Fulfillment_Service.Features.Orders.GetOrderById.Queries;
using System.Globalization;
using System.Security.Claims;

namespace Order___Fulfillment_Service.Features.Orders.GetOrderById
{
    public static class GetOrderByIdEndpoint
    {
        public static IEndpointRouteBuilder MapGetOrderByIdEndpoint(this IEndpointRouteBuilder app)
        {
            var handler = async (
                Guid orderId,
                IMediator mediator,
                ClaimsPrincipal user,
                CancellationToken ct) =>
            {
                var isArabic = CultureInfo.CurrentUICulture.Name.StartsWith("ar");
                var customerIdClaim = user.FindFirstValue(FlowerClaimTypes.CustomerId)
                                      ?? user.FindFirstValue(ClaimTypes.NameIdentifier);

                if (string.IsNullOrEmpty(customerIdClaim) || !Guid.TryParse(customerIdClaim, out var customerId))
                {
                    var msg = "Access denied.";
                    var msgAr = isArabic ? "تم رفض الوصول." : msg;
                    return Results.Json(
                        FloweryApiResponse<OrderDetailDto>.Failure(msg, "Forbidden", msgAr),
                        statusCode: StatusCodes.Status403Forbidden);
                }

                var result = await mediator.Send(
                    new GetOrderByIdQuery(customerId, orderId), ct);

                if (result.IsFailure)
                {
                    var errorMsg = result.Error?.Message ?? "Order not found.";
                    var errorMsgAr = isArabic ? "الطلب غير موجود." : errorMsg;
                    var statusCode = result.Error?.StatusCode switch
                    {
                        StatusCodes.Status404NotFound => StatusCodes.Status404NotFound,
                        StatusCodes.Status403Forbidden => StatusCodes.Status403Forbidden,
                        _ => StatusCodes.Status400BadRequest
                    };
                    var statusName = statusCode switch
                    {
                        StatusCodes.Status404NotFound => "NotFound",
                        StatusCodes.Status403Forbidden => "Forbidden",
                        _ => "BadRequest"
                    };

                    return Results.Json(
                        FloweryApiResponse<OrderDetailDto>.Failure(errorMsg, statusName, errorMsgAr),
                        statusCode: statusCode);
                }

                var successMsg = "Order retrieved successfully.";
                var successMsgAr = isArabic ? "تم استرجاع الطلب بنجاح." : successMsg;

                return Results.Ok(
                    FloweryApiResponse<OrderDetailDto>.Success(result.Value, successMsg, successMsgAr));
            };

            app.MapGet("/orders/{orderId:guid}", handler)
                .WithName("GetOrderById")
                .WithTags("Orders")
                .WithSummary("Get order detail")
                .Produces<FloweryApiResponse<OrderDetailDto>>(StatusCodes.Status200OK)
                .Produces<FloweryApiResponse<OrderDetailDto>>(StatusCodes.Status404NotFound)
                .RequireAuthorization();

            app.MapGet("/api/orders/{orderId:guid}", handler).ExcludeFromDescription().RequireAuthorization();
            app.MapGet("/api/v1/orders/{orderId:guid}", handler).ExcludeFromDescription().RequireAuthorization();

            return app;
        }
    }
}
