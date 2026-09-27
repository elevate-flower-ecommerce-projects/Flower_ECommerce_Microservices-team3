using Blocks.Contracts.Http;
using Blocks.Contracts.Payment;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Payment_Service.Application.Abstractions;
using Payment_Service.Entities;
using Payment_Service.Entities.Enums;
using Payment_Service.Persistence.Repositories;

namespace Payment_Service.Features.Queries.PaymentStatus;

public sealed record PaymentStatusResultDto(
    Guid OrderId,
    string OrderStatus,
    string PaymentStatus,
    DateTime? EstimatedDeliveryAt,
    DateTime UpdatedAt
);

public sealed record RetryCardResultDto(
    Guid OrderId,
    string Status,
    string Gateway,
    string SessionId,
    string SessionUrl,
    string SuccessUrl,
    string CancelUrl,
    DateTime ExpiresAt,
    decimal Amount,
    string Currency,
    DateTime EstimatedDeliveryAt
);

public static class PaymentStatusEndpoint
{
    public static IEndpointRouteBuilder MapPaymentStatusEndpoints(this IEndpointRouteBuilder endpoints)
    {
        // 1. Get Payment / Order Status
        endpoints.MapGet("/payments/orders/{orderId}/status", async (
            Guid orderId,
            IPaymentRepository paymentRepository,
            CancellationToken ct) =>
        {
            var payment = await paymentRepository.GetByOrderIdAsync(orderId, ct);
            if (payment is null)
            {
                return Results.Ok(FloweryApiResponse<PaymentStatusResultDto>.Success(
                    new PaymentStatusResultDto(
                        OrderId: orderId,
                        OrderStatus: "Placed",
                        PaymentStatus: "NotRequired",
                        EstimatedDeliveryAt: null,
                        UpdatedAt: DateTime.UtcNow
                    ),
                    "Payment status retrieved.",
                    "تم جلب حالة الدفع بنجاح."));
            }

            var orderStatus = payment.Status switch
            {
                Payment_Service.Entities.Enums.PaymentStatus.Paid => "Placed",
                Payment_Service.Entities.Enums.PaymentStatus.Failed => "PaymentFailed",
                Payment_Service.Entities.Enums.PaymentStatus.Cancelled => "Cancelled",
                _ => "PendingPayment"
            };

            var paymentStatusStr = payment.Status switch
            {
                Payment_Service.Entities.Enums.PaymentStatus.Paid => "Succeeded",
                Payment_Service.Entities.Enums.PaymentStatus.Failed => "Failed",
                Payment_Service.Entities.Enums.PaymentStatus.Cancelled => "Cancelled",
                _ => "Pending"
            };

            return Results.Ok(FloweryApiResponse<PaymentStatusResultDto>.Success(
                new PaymentStatusResultDto(
                    OrderId: orderId,
                    OrderStatus: orderStatus,
                    PaymentStatus: paymentStatusStr,
                    EstimatedDeliveryAt: null,
                    UpdatedAt: payment.PaidAt ?? DateTime.UtcNow
                ),
                "Payment status retrieved.",
                "تم جلب حالة الدفع بنجاح."));
        })
        .WithName("GetPaymentStatus")
        .WithTags("Payments")
        .Produces<FloweryApiResponse<PaymentStatusResultDto>>(StatusCodes.Status200OK);

        // 2. Retry Order Payment
        endpoints.MapPost("/payments/orders/{orderId}/retry", async (
            Guid orderId,
            IPaymentRepository paymentRepository,
            IPaymentGateway paymentGateway,
            IUnitOfWork unitOfWork,
            CancellationToken ct) =>
        {
            var payment = await paymentRepository.GetByOrderIdAsync(orderId, ct);
            if (payment is not null && payment.Status == Payment_Service.Entities.Enums.PaymentStatus.Paid)
            {
                return Results.Conflict(FloweryApiResponse<RetryCardResultDto>.Failure(
                    "Order is already paid.",
                    "Conflict",
                    "هذا الطلب تم سداده بالفعل."));
            }

            var amount = payment?.Amount ?? 100m;
            var currency = payment?.Currency ?? "EGP";
            var eta = DateTime.UtcNow.AddMinutes(45);

            var sessionResult = await paymentGateway.CreateCheckoutSessionAsync(
                new CreatePaymentSessionRequest(
                    OrderId: orderId,
                    Amount: amount,
                    Currency: currency,
                    Provider: PaymentProvider.Paymob,
                    EstimatedDeliveryAt: eta,
                    BillingData: new BillingData("Customer", "User", "customer@example.com", "+201000000000", "EGY", "Cairo", "Street", "1", "1", "1")
                ),
                ct);

            if (payment is null)
            {
                payment = new Payment
                {
                    OrderId = orderId,
                    Amount = amount,
                    Currency = currency,
                    Method = PaymentMethod.Card,
                    Status = Payment_Service.Entities.Enums.PaymentStatus.Pending,
                    PaymobIntentionId = sessionResult.IntentionId,
                    PaymobClientSecret = sessionResult.ClientSecret
                };
                await paymentRepository.AddAsync(payment, ct);
            }
            else
            {
                payment.PaymobIntentionId = sessionResult.IntentionId;
                payment.PaymobClientSecret = sessionResult.ClientSecret;
                payment.Status = Payment_Service.Entities.Enums.PaymentStatus.Pending;
            }

            await unitOfWork.SaveChangesAsync(ct);

            var retryResult = new RetryCardResultDto(
                OrderId: orderId,
                Status: "PendingPayment",
                Gateway: "Paymob",
                SessionId: sessionResult.IntentionId,
                SessionUrl: sessionResult.PaymentUrl,
                SuccessUrl: $"flowery://payment/success?orderId={orderId}",
                CancelUrl: $"flowery://payment/cancel?orderId={orderId}",
                ExpiresAt: DateTime.UtcNow.AddMinutes(30),
                Amount: amount,
                Currency: currency,
                EstimatedDeliveryAt: eta
            );

            return Results.Ok(FloweryApiResponse<RetryCardResultDto>.Success(
                retryResult,
                "A fresh gateway session was opened for the existing order.",
                "تم فتح جلسة دفع جديدة للطلب بنجاح.",
                "Success"));
        })
        .WithName("RetryOrderPayment")
        .WithTags("Payments")
        .Produces<FloweryApiResponse<RetryCardResultDto>>(StatusCodes.Status200OK)
        .Produces<FloweryApiResponse<RetryCardResultDto>>(StatusCodes.Status409Conflict);

        return endpoints;
    }
}
