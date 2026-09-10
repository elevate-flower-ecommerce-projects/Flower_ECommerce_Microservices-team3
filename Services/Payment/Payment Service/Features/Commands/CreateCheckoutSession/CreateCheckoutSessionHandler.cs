using Blocks.Contracts.Common;
using Blocks.Contracts.Payment;
using Blocks.Domain.Errors;
using MediatR;
using Payment_Service.Application.Abstractions;
using Payment_Service.Entities;
using Payment_Service.Entities.Enums;
using Payment_Service.Persistence.Repositories;

namespace Payment_Service.Features.Commands.CreateCheckoutSession;

public sealed class CreateCheckoutSessionHandler(
    IPaymentRepository paymentRepository,
    IPaymentGateway paymentGateway,
    IUnitOfWork unitOfWork)
    : IRequestHandler<
        CreateCheckoutSessionCommand,
        Result<CreateCheckoutSessionResponse>>
{
    public async Task<Result<CreateCheckoutSessionResponse>> Handle(
        CreateCheckoutSessionCommand request,
        CancellationToken cancellationToken)
    {
        // 1. Check if a payment already exists for this order
        var payment = await paymentRepository.GetByOrderIdAsync(
            request.OrderId,
            cancellationToken);

        // 2. If already paid, don't create another payment
        if (payment is not null &&
            payment.Status == PaymentStatus.Paid)
        {
            return Result.Failure<CreateCheckoutSessionResponse>(
                Error.Conflict(
                    "This order has already been paid."));
        }

        // 3. Create Payment record if it doesn't exist
        if (payment is null)
        {
            payment = new Payment
            {
                OrderId = request.OrderId,
                Amount = request.Amount,
                Currency = request.Currency,
                Method = PaymentMethod.Card,
                Status = PaymentStatus.Pending
            };

            await paymentRepository.AddAsync(
                payment,
                cancellationToken);
        }

        // 4. Create checkout session through the selected provider
        var checkoutSession =
            await paymentGateway.CreateCheckoutSessionAsync(
                new CreatePaymentSessionRequest(
                    request.OrderId,
                    request.Amount,
                    request.Currency,
                    PaymentProvider.Paymob,
                    request.EstimatedDeliveryAt,
                    request.BillingData),
                cancellationToken);

        // 5. Store Paymob identifiers
        payment.PaymobIntentionId =
            checkoutSession.IntentionId;

        payment.PaymobClientSecret =
            checkoutSession.ClientSecret;

        payment.PaymobOrderId =
            checkoutSession.PaymobOrderId;

        // 6. Save Payment
        await unitOfWork.SaveChangesAsync(
            cancellationToken);

        // 7. Return checkout information
        return Result<CreateCheckoutSessionResponse>.Success(
            new CreateCheckoutSessionResponse(
                payment.OrderId,
                payment.Id,
                checkoutSession.PaymentUrl));
    }
}