using Blocks.Contracts.Http;
using Blocks.Contracts.Security;
using Blocks.Domain.Errors;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using System.Globalization;
using System.Security.Claims;

namespace Order___Fulfillment_Service.Features.Checkout.GetCheckoutDetails;

public static class GetCheckoutDetailsEndpoint
{
    public static IEndpointRouteBuilder MapGetCheckoutDetailsEndpoint(this IEndpointRouteBuilder app)
    {
        var handler = async (
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
                    FloweryApiResponse<CheckoutDetailsResponse>.Failure(msg, "Unauthorized", msgAr),
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

            var result = await sender.Send(new GetCheckoutDetailsQuery(customerId, bearerToken, cartId), cancellationToken);

            if (result.IsSuccess)
            {
                var successMsg = "Checkout preview generated successfully.";
                var successMsgAr = isArabic ? "تم جلب تفاصيل إتمام الطلب بنجاح." : successMsg;
                return Results.Ok(FloweryApiResponse<CheckoutDetailsResponse>.Success(result.Value, successMsg, successMsgAr));
            }

            var errorMsg = result.Error?.Message ?? "Failed to retrieve checkout details.";
            var errorMsgAr = isArabic ? "فشل في جلب تفاصيل إتمام الطلب." : errorMsg;
            var statusCode = result.Error?.StatusCode switch
            {
                null or 0 => StatusCodes.Status400BadRequest,
                var code => code
            };

            return Results.Json(
                FloweryApiResponse<CheckoutDetailsResponse>.Failure(
                    errorMsg,
                    statusCode == 404 ? "NotFound" : "BadRequest",
                    errorMsgAr),
                statusCode: statusCode);
        };

        app.MapGet("/checkout/details", handler)
            .WithName("GetCheckoutDetails")
            .WithTags("Checkout")
            .WithSummary("Get Checkout Screen Preview")
            .WithDescription("Returns checkout preview: pricing for the cart against user's default/selected address, ETA, and payment methods.")
            .Produces<FloweryApiResponse<CheckoutDetailsResponse>>(StatusCodes.Status200OK)
            .Produces<FloweryApiResponse<CheckoutDetailsResponse>>(StatusCodes.Status400BadRequest)
            .Produces<FloweryApiResponse<CheckoutDetailsResponse>>(StatusCodes.Status401Unauthorized)
            .RequireAuthorization();

        app.MapGet("/api/checkout/details", handler).ExcludeFromDescription().RequireAuthorization();

        return app;
    }
}
