using Blocks.Contracts.Common;
using Blocks.Domain.Errors;
using MediatR;
using Payment_Service.Application.Abstractions;
using Payment_Service.Entities;
using Payment_Service.Entities.Enums;
using Payment_Service.Persistence.Repositories;

namespace Payment_Service.Features.Commands.CreateCodPayment;

public sealed class CreateCodPaymentHandler(
    IPaymentRepository paymentRepository,
    IUnitOfWork unitOfWork)
    : IRequestHandler<
        CreateCodPaymentCommand,
        Result<CreateCodPaymentResponse>>
{
    public async Task<Result<CreateCodPaymentResponse>> Handle(
        CreateCodPaymentCommand request,
        CancellationToken cancellationToken)
    {
        // Check if a payment already exists for this order
        var existingPayment =
            await paymentRepository.GetByOrderIdAsync(
                request.OrderId,
                cancellationToken);

        if (existingPayment is not null)
        {
            return Result.Failure<CreateCodPaymentResponse>(
                Error.Conflict(
                    "A payment already exists for this order."));
        }

        // Create COD payment
        var payment = new Payment
        {
            OrderId = request.OrderId,
            Amount = request.Amount,
            Currency = request.Currency,
            Method = PaymentMethod.CashOnDelivery,
            Status = PaymentStatus.Pending
        };

        await paymentRepository.AddAsync(
            payment,
            cancellationToken);

        // Save
        await unitOfWork.SaveChangesAsync(
            cancellationToken);

        return Result<CreateCodPaymentResponse>.Success(
            new CreateCodPaymentResponse(
                payment.OrderId,
                payment.Id,
                payment.Status));
    }
}