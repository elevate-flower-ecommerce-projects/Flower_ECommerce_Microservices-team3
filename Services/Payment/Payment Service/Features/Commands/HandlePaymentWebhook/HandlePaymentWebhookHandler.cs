using Blocks.Contracts.Common;
using Blocks.Domain.Errors;
using MediatR;
using Payment_Service.Application.Abstractions;
using Payment_Service.Entities.Enums;
using Payment_Service.Persistence.Repositories;

namespace Payment_Service.Features.Commands.HandlePaymentWebhook;

public sealed class HandlePaymentWebhookHandler(
    IPaymentRepository paymentRepository,
    IUnitOfWork unitOfWork)
    : IRequestHandler<HandlePaymentWebhookCommand, Result>
{
    public async Task<Result> Handle(
        HandlePaymentWebhookCommand request,
        CancellationToken cancellationToken)
    {
        var payment =
            await paymentRepository.GetByPaymobIntentionIdAsync(
                request.IntentionId,
                cancellationToken);

        if (payment is null)
        {
            return Result.Failure(
                Error.NotFound("Payment not found."));
        }

        // Ignore duplicate webhook for the same transaction
        if (!string.IsNullOrWhiteSpace(payment.PaymobTransactionId) &&
            payment.PaymobTransactionId == request.TransactionId)
        {
            return Result.Success();
        }

        payment.PaymobTransactionId = request.TransactionId;

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

        await unitOfWork.SaveChangesAsync(
            cancellationToken);

        return Result.Success();
    }
}