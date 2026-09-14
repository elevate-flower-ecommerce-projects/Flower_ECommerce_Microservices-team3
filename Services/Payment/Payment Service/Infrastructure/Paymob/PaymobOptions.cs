namespace Payment_Service.Integrations.Paymob
{
    public sealed class PaymobOptions
    {
        public const string SectionName = "Paymob";
        public string BaseUrl { get; set; } = string.Empty;
        public string SecretKey { get; set; } = string.Empty;
        public string PublicKey { get; set; } = string.Empty;
        public string ApiKey { get; set; } = string.Empty;
        public int IntegrationId { get; set; }
        public string NotificationUrl { get; set; } = string.Empty;
    }
}
