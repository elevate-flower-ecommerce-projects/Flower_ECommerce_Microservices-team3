namespace Payment_Service.Infrastructure.Paymob
{
    public sealed class PaymobOptions
    {
        public const string SectionName = "Paymob";
        public string BaseUrl { get; set; } = "https://accept.paymob.com/api/";
        public string SecretKey { get; set; } = string.Empty;
        public string PublicKey { get; set; } = string.Empty;
        public string ApiKey { get; set; } = string.Empty;
        public string? HmacSecret { get; set; }
        public string Currency { get; set; } = "EGP";
        public int IntegrationId { get; set; }
        public string NotificationUrl { get; set; } = string.Empty;
    }
}
