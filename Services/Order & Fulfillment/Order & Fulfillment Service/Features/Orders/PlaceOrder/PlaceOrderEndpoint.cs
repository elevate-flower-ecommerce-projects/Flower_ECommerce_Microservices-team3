using System.Globalization;
using System.Security.Claims;
using Blocks.Contracts.Http;
using Blocks.Contracts.Security;
using Blocks.Domain.Errors;
using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Order___Fulfillment_Service.Features.Orders.PlaceOrder.Commands;
using Order___Fulfillment_Service.Features.Orders.PlaceOrder.DTOs;

namespace Order___Fulfillment_Service.Features.Orders.PlaceOrder;

public static class PlaceOrderEndpoint
{
    public static IEndpointRouteBuilder MapPlaceOrderEndpoint(this IEndpointRouteBuilder app)
    {
        var handler = async (
            PlaceOrderRequest request,
            IMediator mediator,
            ClaimsPrincipal user,
            HttpContext httpContext,
            CancellationToken ct) =>
        {
            var isArabic = CultureInfo.CurrentUICulture.Name.StartsWith("ar");
            var customerIdClaim = user.FindFirstValue(FlowerClaimTypes.CustomerId)
                ?? user.FindFirstValue(ClaimTypes.NameIdentifier);

            if (string.IsNullOrEmpty(customerIdClaim) || !Guid.TryParse(customerIdClaim, out var customerId))
            {
                var msg = "You are not authorized to access this resource.";
                var msgAr = isArabic ? "غير مصرح لك بالوصول إلى هذا المورد." : msg;
                return Results.Json(
                    FloweryApiResponse<PlaceOrderCardResult?>.Failure(msg, "Unauthorized", msgAr),
                    statusCode: StatusCodes.Status401Unauthorized);
            }

            var authHeader = httpContext.Request.Headers.Authorization.ToString();
            var bearerToken = authHeader.StartsWith("Bearer ", StringComparison.OrdinalIgnoreCase)
                ? authHeader[7..].Trim()
                : authHeader;

            var email = user.FindFirstValue(ClaimTypes.Email) ?? "customer@example.com";
            var name = user.FindFirstValue(ClaimTypes.Name) ?? "Valued Customer";
            var phone = !string.IsNullOrWhiteSpace(request.CustomerPhone)
                ? request.CustomerPhone
                : (user.FindFirstValue(ClaimTypes.MobilePhone) ?? user.FindFirstValue("phone") ?? string.Empty);

            var command = new PlaceOrderCommand(
                CustomerId: customerId,
                CustomerEmail: email,
                CustomerName: name,
                CustomerPhone: phone,
                BearerToken: bearerToken,
                Request: request
            );

            var result = await mediator.Send(command, ct);
            if (result.IsFailure)
            {
                var errorMsg = result.Error?.Message ?? "Failed to place order.";
                var errorMsgAr = isArabic ? "فشل في تأكيد الطلب." : errorMsg;
                var statusCode = result.Error?.StatusCode switch
                {
                    null or 0 => StatusCodes.Status400BadRequest,
                    var code => code
                };

                return Results.Json(
                    FloweryApiResponse<PlaceOrderCardResult?>.Failure(
                        errorMsg,
                        statusCode == 404 ? "NotFound" : (statusCode == 409 ? "Conflict" : "ValidationError"),
                        errorMsgAr),
                    statusCode: statusCode);
            }

            // Success response:
            // For COD: result.Value is null
            // For Card: result.Value is PlaceOrderCardResult
            if (result.Value is null)
            {
                var codMsg = "Order placed successfully.";
                var codMsgAr = isArabic ? "تم تأكيد طلبك بنجاح." : codMsg;
                return Results.Json(
                    FloweryApiResponse<PlaceOrderCardResult?>.Success(null, codMsg, codMsgAr, "Created"),
                    statusCode: StatusCodes.Status201Created);
            }

            var cardMsg = "Order created. Please proceed to payment.";
            var cardMsgAr = isArabic ? "تم إنشاء الطلب. يرجى المتابعة للدفع." : cardMsg;
            return Results.Json(
                FloweryApiResponse<PlaceOrderCardResult?>.Success(result.Value, cardMsg, cardMsgAr, "Created"),
                statusCode: StatusCodes.Status201Created);
        };

        // Primary OpenAPI 3.0.3 route
        app.MapPost("/orders/place", handler)
            .WithName("PlaceOrder")
            .WithTags("Orders")
            .WithSummary("Place an order — Cash on Delivery or Card")
            .WithDescription("Single entry point for both payment methods. For COD, data is null. For Card, data contains hosted session details.")
            .Produces<FloweryApiResponse<PlaceOrderCardResult>>(StatusCodes.Status201Created)
            .Produces<FloweryApiResponse<object>>(StatusCodes.Status400BadRequest)
            .Produces<FloweryApiResponse<object>>(StatusCodes.Status401Unauthorized)
            .Produces<FloweryApiResponse<object>>(StatusCodes.Status404NotFound)
            .RequireAuthorization();

        // Convenience & backward-compatible aliases
        app.MapPost("/orders", handler).ExcludeFromDescription().RequireAuthorization();
        app.MapPost("/api/orders", handler).ExcludeFromDescription().RequireAuthorization();
        app.MapPost("/api/v1/orders", handler).ExcludeFromDescription().RequireAuthorization();

        return app;
    }
}
