using Payment_Service.Infrastructure.Paymob;

namespace Payment_Service.Application.Abstractions;

public record PaymobInquiryResult(bool Success, string? TransactionId, string? OrderId, decimal Amount = 0);

public interface IPaymobClient
{
    Task<PaymobIntentionResponse> CreateIntentionAsync(
        PaymobIntentionRequest request,
        CancellationToken cancellationToken = default);

    Task<PaymobInquiryResult?> InquireTransactionAsync(
        string paymobOrderId,
        CancellationToken cancellationToken = default);
}