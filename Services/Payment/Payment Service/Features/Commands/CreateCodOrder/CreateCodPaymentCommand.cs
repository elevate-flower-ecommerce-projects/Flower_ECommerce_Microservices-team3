using Blocks.Contracts.Common;
using MediatR;

namespace Payment_Service.Features.Commands.CreateCodPayment;

public sealed record CreateCodPaymentCommand(
    Guid OrderId,
    decimal Amount,
    string Currency
) : IRequest<Result<CreateCodPaymentResponse>>;