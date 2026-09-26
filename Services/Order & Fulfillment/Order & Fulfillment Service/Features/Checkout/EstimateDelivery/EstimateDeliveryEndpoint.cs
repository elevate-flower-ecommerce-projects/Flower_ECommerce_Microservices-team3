using Blocks.Contracts.Http;
using Blocks.Contracts.Security;
using Blocks.Domain.Errors;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using System.Globalization;
using System.Security.Claims;

namespace Order___Fulfillment_Service.Features.Checkout.EstimateDelivery;

public static class EstimateDeliveryEndpoint
{
    public static IEndpointRouteBuilder MapEstimateDeliveryEndpoint(this IEndpointRouteBuilder app)
    {
        var handler = async (
            [FromQuery] Guid addressId,
            [FromQuery] Guid? cartId,
            ISender sender,
            ClaimsPrincipal user,
            HttpContext httpContext,
            CancellationToken cancellationToken) =>
        {
            var isArabic = CultureInfo.CurrentUICulture.Name.StartsWith("ar");
            var customerIdClaim = user.FindFirstValue(FlowerClaimTypes.CustomerId)
                                  ?? user.FindFirstValue(ClaimTypes.NameIdentifier);

            if (string.IsNullOrEmpty(customerIdClaim) || !Guid.TryParse(customerIdClaim, out var customerId))
            {
                var msg = "You are not authorized to access this resource.";
                var msgAr = isArabic ? "غير مصرح لك بالوصول إلى هذا المورد." : msg;
                return Results.Json(
                    FloweryApiResponse<EstimateDeliveryResponse>.Failure(msg, "Unauthorized", msgAr),
                    statusCode: StatusCodes.Status401Unauthorized);
            }

            string? bearerToken = null;
            if (httpContext.Request.Headers.TryGetValue("Authorization", out var authHeader))
            {
                var headerStr = authHeader.ToString();
                if (headerStr.StartsWith("Bearer ", StringComparison.OrdinalIgnoreCase))
                {
                    bearerToken = headerStr["Bearer ".Length..].Trim();
                }
                else
                {
                    bearerToken = headerStr.Trim();
                }
            }

            var result = await sender.Send(new EstimateDeliveryQuery(customerId, addressId, bearerToken, cartId), cancellationToken);

            if (result.IsSuccess)
            {
                var successMsg = "Delivery estimated successfully.";
                var successMsgAr = isArabic ? "تم تقدير وقت التوصيل بنجاح." : successMsg;
                return Results.Ok(FloweryApiResponse<EstimateDeliveryResponse>.Success(result.Value, successMsg, successMsgAr));
            }

            var errorMsg = result.Error?.Message ?? "Failed to estimate delivery.";
            var errorMsgAr = isArabic ? "فشل في تقدير وقت التوصيل." : errorMsg;
            var statusCode = result.Error?.StatusCode switch
            {
                null or 0 => StatusCodes.Status400BadRequest,
                var code => code
            };

            return Results.Json(
                FloweryApiResponse<EstimateDeliveryResponse>.Failure(
                    errorMsg,
                    statusCode == 404 ? "NotFound" : "BadRequest",
                    errorMsgAr),
                statusCode: statusCode);
        };

        app.MapGet("/checkout/estimate-delivery", handler)
            .WithName("EstimateDelivery")
            .WithTags("Checkout")
            .WithSummary("Recompute Delivery Estimate for a Different Address")
            .WithDescription("Recomputes ETA and delivery fee when switching selected address on the checkout screen.")
            .Produces<FloweryApiResponse<EstimateDeliveryResponse>>(StatusCodes.Status200OK)
            .Produces<FloweryApiResponse<EstimateDeliveryResponse>>(StatusCodes.Status400BadRequest)
            .Produces<FloweryApiResponse<EstimateDeliveryResponse>>(StatusCodes.Status401Unauthorized)
            .Produces<FloweryApiResponse<EstimateDeliveryResponse>>(StatusCodes.Status404NotFound)
            .RequireAuthorization();

        app.MapGet("/api/checkout/estimate-delivery", handler).ExcludeFromDescription().RequireAuthorization();

        return app;
    }
}
