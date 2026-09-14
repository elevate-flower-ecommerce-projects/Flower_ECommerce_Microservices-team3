using Blocks.Contracts.Common;
using Blocks.Contracts.Payment;
using MediatR;

namespace Payment_Service.Features.Commands.CreateCheckoutSession;

public sealed record CreateCheckoutSessionCommand(
    Guid OrderId,
    decimal Amount,
    string Currency,
    DateTime EstimatedDeliveryAt,
    BillingData BillingData
) : IRequest<Result<CreateCheckoutSessionResponse>>;