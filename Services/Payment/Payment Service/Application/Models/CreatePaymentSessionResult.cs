namespace Payment_Service.Application.Models
{
    public sealed record CreatePaymentSessionResult(
        string IntentionId,
        string ClientSecret,
        string PaymobOrderId,
        string PaymentUrl
    );
}
