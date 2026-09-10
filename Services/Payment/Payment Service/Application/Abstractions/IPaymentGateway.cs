using Blocks.Contracts.Payment;
using Payment_Service.Application.Models;

namespace Payment_Service.Application.Abstractions;

public interface IPaymentGateway
{
    Task<CreatePaymentSessionResult> CreateCheckoutSessionAsync(
        CreatePaymentSessionRequest request,
        CancellationToken cancellationToken = default);
}