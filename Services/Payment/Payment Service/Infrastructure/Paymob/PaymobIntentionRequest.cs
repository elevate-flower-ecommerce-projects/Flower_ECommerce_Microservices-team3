using System.Text.Json.Serialization;

namespace Payment_Service.Infrastructure.Paymob;

public sealed class PaymobIntentionRequest
{
    [JsonPropertyName("amount")]
    public int Amount { get; set; }

    [JsonPropertyName("currency")]
    public string Currency { get; set; } = "EGP";

    [JsonPropertyName("special_reference")]
    public string SpecialReference { get; set; } = string.Empty;

    [JsonPropertyName("payment_methods")]
    public List<int> PaymentMethods { get; set; } = [];

    [JsonPropertyName("items")]
    public List<PaymobItem> Items { get; set; } = [];

    [JsonPropertyName("billing_data")]
    public PaymobBillingData BillingData { get; set; } = null!;
}

public sealed class PaymobItem
{
    [JsonPropertyName("name")]
    public string Name { get; set; } = string.Empty;

    [JsonPropertyName("amount")]
    public int Amount { get; set; }

    [JsonPropertyName("quantity")]
    public int Quantity { get; set; } = 1;

    [JsonPropertyName("description")]
    public string? Description { get; set; }
}