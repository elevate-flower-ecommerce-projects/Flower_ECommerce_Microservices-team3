using Microsoft.Extensions.Options;
using Payment_Service.Application.Abstractions;

namespace Payment_Service.Infrastructure.Paymob;

public sealed class PaymobCheckoutUrlBuilder(IOptions<PaymobOptions> options) : IPaymobCheckoutUrlBuilder
{
    private const string CheckoutBaseUrl =
        "https://accept.paymob.com/unifiedcheckout/";
    private readonly PaymobOptions _options = options.Value;

    public string Build(string clientSecret)
    {
        if (string.IsNullOrWhiteSpace(clientSecret))
            throw new ArgumentException(
                "Client secret cannot be empty.",
                nameof(clientSecret));

        var escapedSecret = Uri.EscapeDataString(clientSecret);
        if (!string.IsNullOrWhiteSpace(_options.PublicKey))
        {
            var escapedPublicKey = Uri.EscapeDataString(_options.PublicKey);
            return $"{CheckoutBaseUrl}?publicKey={escapedPublicKey}&clientSecret={escapedSecret}";
        }

        return $"{CheckoutBaseUrl}?clientSecret={escapedSecret}";
    }
}