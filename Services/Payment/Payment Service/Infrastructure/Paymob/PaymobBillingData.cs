using System.Text.Json.Serialization;

namespace Payment_Service.Infrastructure.Paymob
{
    public sealed class PaymobBillingData
    {
        [JsonPropertyName("first_name")]
        public string FirstName { get; set; } = string.Empty;

        [JsonPropertyName("last_name")]
        public string LastName { get; set; } = string.Empty;

        [JsonPropertyName("email")]
        public string Email { get; set; } = string.Empty;

        [JsonPropertyName("phone_number")]
        public string PhoneNumber { get; set; } = string.Empty;

        [JsonPropertyName("country")]
        public string Country { get; set; } = string.Empty;

        [JsonPropertyName("city")]
        public string City { get; set; } = string.Empty;

        [JsonPropertyName("street")]
        public string Street { get; set; } = string.Empty;

        [JsonPropertyName("building")]
        public string Building { get; set; } = string.Empty;

        [JsonPropertyName("floor")]
        public string Floor { get; set; } = string.Empty;

        [JsonPropertyName("apartment")]
        public string Apartment { get; set; } = string.Empty;
    }
}
