namespace Payment_Service.Infrastructure.Paymob;

public sealed class PaymobIntentionRequest
{
    public int Amount { get; set; }
    public string Currency { get; set; } = "EGP";
    public string SpecialReference { get; set; } = string.Empty;
    public List<int> PaymentMethods { get; set; } = [];
    public PaymobBillingData BillingData { get; set; } = null!;
}