using Payment_Service.Infrastructure.Paymob;

public interface IPaymobClient
{
    Task<PaymobIntentionResponse> CreateIntentionAsync(
        PaymobIntentionRequest request,
        CancellationToken cancellationToken = default);
}