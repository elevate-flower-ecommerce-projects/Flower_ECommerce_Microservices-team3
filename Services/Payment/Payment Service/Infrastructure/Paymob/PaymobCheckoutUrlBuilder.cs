using Payment_Service.Application.Abstractions;

namespace Payment_Service.Infrastructure.Paymob;

public sealed class PaymobCheckoutUrlBuilder : IPaymobCheckoutUrlBuilder
{
    private const string CheckoutBaseUrl =
        "https://accept.paymob.com/unifiedcheckout/";

    public string Build(string clientSecret)
    {
        if (string.IsNullOrWhiteSpace(clientSecret))
            throw new ArgumentException(
                "Client secret cannot be empty.",
                nameof(clientSecret));

        return $"{CheckoutBaseUrl}?client_secret={Uri.EscapeDataString(clientSecret)}";
    }
}