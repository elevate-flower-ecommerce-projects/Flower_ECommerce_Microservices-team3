using Blocks.Contracts.Http;
using Blocks.Contracts.Payment;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
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

public sealed record ConfirmPaymentRequestDto(
    bool Success = true,
    string? TransactionId = null
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
        // 1. Get Payment / Order Status (with optional sync from Paymob redirect query params)
        endpoints.MapGet("/payments/orders/{orderId}/status", async (
            Guid orderId,
            [FromQuery] bool? success,
            [FromQuery] string? transactionId,
            [FromQuery] bool? sync,
            IPaymentRepository paymentRepository,
            IPaymobClient paymobClient,
            IOrderServiceClient orderServiceClient,
            IUnitOfWork unitOfWork,
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

            // If payment is pending, check if client provided success status from redirect or inquire Paymob
            if (payment.Status == Payment_Service.Entities.Enums.PaymentStatus.Pending)
            {
                if (success == true)
                {
                    payment.Status = Payment_Service.Entities.Enums.PaymentStatus.Paid;
                    payment.PaidAt = DateTime.UtcNow;
                    if (!string.IsNullOrWhiteSpace(transactionId))
                    {
                        payment.PaymobTransactionId = transactionId;
                    }

                    paymentRepository.Update(payment);
                    await unitOfWork.SaveChangesAsync(ct);

                    await orderServiceClient.MarkOrderAsPaidAsync(orderId, ct);
                }
                else
                {
                    var inquiryTarget = !string.IsNullOrWhiteSpace(payment.PaymobOrderId)
                        ? payment.PaymobOrderId
                        : orderId.ToString();

                    var inquiry = await paymobClient.InquireTransactionAsync(inquiryTarget, ct);
                    if (inquiry is not null && inquiry.Success)
                    {
                        payment.Status = Payment_Service.Entities.Enums.PaymentStatus.Paid;
                        payment.PaidAt = DateTime.UtcNow;
                        if (!string.IsNullOrWhiteSpace(inquiry.TransactionId))
                        {
                            payment.PaymobTransactionId = inquiry.TransactionId;
                        }

                        paymentRepository.Update(payment);
                        await unitOfWork.SaveChangesAsync(ct);

                        await orderServiceClient.MarkOrderAsPaidAsync(orderId, ct);
                    }
                }
            }

            var orderStatus = payment.Status switch
            {
                Payment_Service.Entities.Enums.PaymentStatus.Paid => "Preparing",
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

        // 2. Explicit Confirm Payment Endpoint (called by mobile app or frontend after gateway return)
        endpoints.MapPost("/payments/orders/{orderId}/confirm", async (
            Guid orderId,
            [FromBody] ConfirmPaymentRequestDto? body,
            IPaymentRepository paymentRepository,
            IOrderServiceClient orderServiceClient,
            IUnitOfWork unitOfWork,
            CancellationToken ct) =>
        {
            var payment = await paymentRepository.GetByOrderIdAsync(orderId, ct);
            if (payment is null)
            {
                return Results.NotFound(FloweryApiResponse<PaymentStatusResultDto>.Failure(
                    "Payment record not found for this order.",
                    "NotFound",
                    "لم يتم العثور على سجل الدفع لهذا الطلب."));
            }

            var isSuccess = body?.Success ?? true;
            if (isSuccess)
            {
                payment.Status = Payment_Service.Entities.Enums.PaymentStatus.Paid;
                payment.PaidAt = DateTime.UtcNow;
                if (!string.IsNullOrWhiteSpace(body?.TransactionId))
                {
                    payment.PaymobTransactionId = body.TransactionId;
                }

                paymentRepository.Update(payment);
                await unitOfWork.SaveChangesAsync(ct);

                await orderServiceClient.MarkOrderAsPaidAsync(orderId, ct);
            }
            else
            {
                payment.Status = Payment_Service.Entities.Enums.PaymentStatus.Failed;
                paymentRepository.Update(payment);
                await unitOfWork.SaveChangesAsync(ct);
            }

            return Results.Ok(FloweryApiResponse<PaymentStatusResultDto>.Success(
                new PaymentStatusResultDto(
                    OrderId: orderId,
                    OrderStatus: isSuccess ? "Preparing" : "PaymentFailed",
                    PaymentStatus: isSuccess ? "Succeeded" : "Failed",
                    EstimatedDeliveryAt: null,
                    UpdatedAt: payment.PaidAt ?? DateTime.UtcNow
                ),
                "Payment confirmed successfully.",
                "تم تأكيد حالة الدفع بنجاح."));
        })
        .WithName("ConfirmOrderPayment")
        .WithTags("Payments")
        .Produces<FloweryApiResponse<PaymentStatusResultDto>>(StatusCodes.Status200OK)
        .Produces<FloweryApiResponse<PaymentStatusResultDto>>(StatusCodes.Status404NotFound);

        // 3. Browser Return / Callback Gateway Endpoint
        endpoints.MapGet("/payments/callback", async (
            [FromQuery] Guid? orderId,
            [FromQuery] bool? success,
            [FromQuery] string? id,
            [FromQuery] string? transaction_id,
            [FromQuery(Name = "order")] string? paymobOrder,
            IPaymentRepository paymentRepository,
            IOrderServiceClient orderServiceClient,
            IUnitOfWork unitOfWork,
            CancellationToken ct) =>
        {
            var isSuccess = success ?? true;
            var txnId = !string.IsNullOrWhiteSpace(id) ? id : transaction_id;

            Payment? payment = null;
            if (orderId.HasValue)
            {
                payment = await paymentRepository.GetByOrderIdAsync(orderId.Value, ct);
            }

            if (payment is null && !string.IsNullOrWhiteSpace(paymobOrder))
            {
                payment = await paymentRepository.GetByPaymobOrderIdAsync(paymobOrder, ct);
                if (payment is not null)
                {
                    orderId = payment.OrderId;
                }
            }

            if (payment is not null && isSuccess && payment.Status != Payment_Service.Entities.Enums.PaymentStatus.Paid)
            {
                payment.Status = Payment_Service.Entities.Enums.PaymentStatus.Paid;
                payment.PaidAt = DateTime.UtcNow;
                if (!string.IsNullOrWhiteSpace(txnId))
                {
                    payment.PaymobTransactionId = txnId;
                }

                paymentRepository.Update(payment);
                await unitOfWork.SaveChangesAsync(ct);

                await orderServiceClient.MarkOrderAsPaidAsync(payment.OrderId, ct);
            }

            var resolvedOrderId = orderId ?? payment?.OrderId;
            var redirectTarget = isSuccess && resolvedOrderId.HasValue
                ? $"flowery://payment/success?orderId={resolvedOrderId.Value}&id={txnId}"
                : $"flowery://payment/cancel?orderId={resolvedOrderId}&id={txnId}";

            var html = $$"""
            <!DOCTYPE html>
            <html lang="en">
            <head>
                <meta charset="utf-8">
                <meta name="viewport" content="width=device-width, initial-scale=1.0">
                <title>{{(isSuccess ? "Payment Successful" : "Payment Cancelled")}}</title>
                <style>
                    body { font-family: -apple-system, BlinkMacSystemFont, 'Segoe UI', Roboto, Helvetica, Arial, sans-serif; display: flex; align-items: center; justify-content: center; min-height: 100vh; margin: 0; background: #f8fafc; }
                    .card { background: white; padding: 40px; border-radius: 20px; box-shadow: 0 10px 25px rgba(0,0,0,0.05); text-align: center; max-width: 420px; width: 90%; }
                    .icon { font-size: 64px; margin-bottom: 20px; color: {{(isSuccess ? "#10B981" : "#EF4444")}}; }
                    h1 { margin: 0 0 12px; color: #1e293b; font-size: 24px; }
                    p { margin: 0 0 28px; color: #64748b; font-size: 15px; line-height: 1.5; }
                    .btn { display: inline-block; background: #D946EF; color: white; padding: 14px 28px; border-radius: 12px; text-decoration: none; font-weight: 600; font-size: 15px; }
                </style>
            </head>
            <body>
                <div class="card">
                    <div class="icon">{{(isSuccess ? "✓" : "✗")}}</div>
                    <h1>{{(isSuccess ? "Payment Successful!" : "Payment Cancelled")}}</h1>
                    <p>{{(isSuccess ? "Your payment was processed successfully. Returning to the Flowery app..." : "The payment was not completed. You can try again from the app.")}}</p>
                    <a class="btn" href="{{redirectTarget}}">Return to Flowery App</a>
                </div>
                <script>
                    setTimeout(function() {
                        window.location.href = "{{redirectTarget}}";
                    }, 800);
                </script>
            </body>
            </html>
            """;

            return Results.Content(html, "text/html");
        })
        .WithName("PaymentCallback")
        .WithTags("Payments");

        // 4. Retry Order Payment
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
                    BillingData: new BillingData("Customer", "User", "customer@example.com", "+201000000000", "EGY", "Cairo", "Street", "1", "1", "1"),
                    RedirectionUrl: $"flowery://payment/success?orderId={orderId}"
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
