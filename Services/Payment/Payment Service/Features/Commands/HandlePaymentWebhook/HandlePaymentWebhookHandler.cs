using Blocks.Contracts.Common;
using Blocks.Domain.Errors;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Payment_Service.Application.Abstractions;
using Payment_Service.Entities;
using Payment_Service.Entities.Enums;
using Payment_Service.Persistence;
using Payment_Service.Persistence.Repositories;

namespace Payment_Service.Features.Commands.HandlePaymentWebhook;

public sealed class HandlePaymentWebhookHandler(
    IPaymentRepository paymentRepository,
    FlowersPaymentDbContext dbContext,
    IUnitOfWork unitOfWork)
    : IRequestHandler<HandlePaymentWebhookCommand, Result>
{
    public async Task<Result> Handle(
        HandlePaymentWebhookCommand request,
        CancellationToken cancellationToken)
    {
        // 1. Idempotency check on EventId if present
        if (!string.IsNullOrWhiteSpace(request.EventId))
        {
            var existingEvent = await dbContext.PaymentWebhookEvents
                .AnyAsync(e => e.EventId == request.EventId, cancellationToken);

            if (existingEvent)
            {
                return Result.Success();
            }
        }

        // 2. Find payment by Intention ID, Paymob Order ID, or system OrderId
        Payment? payment = null;

        if (!string.IsNullOrWhiteSpace(request.IntentionId))
        {
            payment = await paymentRepository.GetByPaymobIntentionIdAsync(
                request.IntentionId,
                cancellationToken);

            payment ??= await paymentRepository.GetByPaymobOrderIdAsync(
                request.IntentionId,
                cancellationToken);

            if (payment is null && Guid.TryParse(request.IntentionId, out var parsedOrderId))
            {
                payment = await paymentRepository.GetByOrderIdAsync(
                    parsedOrderId,
                    cancellationToken);
            }
        }

        if (payment is null && !string.IsNullOrWhiteSpace(request.TransactionId))
        {
            payment = await paymentRepository.GetByPaymobTransactionIdAsync(
                request.TransactionId,
                cancellationToken);
        }

        if (payment is null)
        {
            return Result.Failure(
                Error.NotFound("Payment not found."));
        }

        // 3. Ignore duplicate webhook for the same transaction if already processed
        if (!string.IsNullOrWhiteSpace(payment.PaymobTransactionId) &&
            payment.PaymobTransactionId == request.TransactionId &&
            payment.Status == PaymentStatus.Paid)
        {
            return Result.Success();
        }

        if (!string.IsNullOrWhiteSpace(request.TransactionId))
        {
            payment.PaymobTransactionId = request.TransactionId;
        }

        if (request.Success)
        {
            payment.Status = PaymentStatus.Paid;
            payment.PaidAt = DateTime.UtcNow;
        }
        else if (request.Cancelled)
        {
            payment.Status = PaymentStatus.Cancelled;
        }
        else if (!request.Pending)
        {
            payment.Status = PaymentStatus.Failed;
        }

        paymentRepository.Update(payment);

        // 4. Record event
        if (!string.IsNullOrWhiteSpace(request.EventId))
        {
            await dbContext.PaymentWebhookEvents.AddAsync(
                new PaymentWebhookEvent
                {
                    EventId = request.EventId,
                    EventType = request.EventType,
                    ReceivedAt = DateTime.UtcNow,
                    ProcessedAt = DateTime.UtcNow
                },
                cancellationToken);
        }

        await unitOfWork.SaveChangesAsync(
            cancellationToken);

        return Result.Success();
    }
}