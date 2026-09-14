using System.Text.Json.Serialization;

namespace Payment_Service.Infrastructure.Paymob
{
    public sealed class PaymobIntentionResponse
    {
        [JsonPropertyName("id")]
        public string Id { get; set; } = string.Empty;

        [JsonPropertyName("client_secret")]
        public string ClientSecret { get; set; } = string.Empty;

        [JsonPropertyName("order")]
        public PaymobOrderResponse? Order { get; set; }
    }

    public sealed class PaymobOrderResponse
    {
        [JsonPropertyName("id")]
        public long Id { get; set; }
    }
}
