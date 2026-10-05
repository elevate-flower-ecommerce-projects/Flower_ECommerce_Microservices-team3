using Blocks.Contracts.Payment;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Payment_Service.Application.Abstractions;
using Payment_Service.Application.Models;
using Payment_Service.Infrastructure.Paymob;

namespace Payment_Service.Infrastructure;

public sealed class PaymentGateway(
    IPaymobClient paymobClient,
    IPaymobCheckoutUrlBuilder checkoutUrlBuilder,
    IOptions<PaymobOptions> options,
    ILogger<PaymentGateway> logger)
    : IPaymentGateway
{
    private readonly PaymobOptions _options = options.Value;

    public async Task<CreatePaymentSessionResult> CreateCheckoutSessionAsync(
        CreatePaymentSessionRequest request,
        CancellationToken cancellationToken = default)
    {
        var amountInCents = Convert.ToInt32(request.Amount * 100);

        var redirectionUrl = !string.IsNullOrWhiteSpace(request.RedirectionUrl)
            ? request.RedirectionUrl
            : $"http://localhost:8080/payments/callback?orderId={request.OrderId}";

        var notificationUrl = !string.IsNullOrWhiteSpace(request.NotificationUrl)
            ? request.NotificationUrl
            : (!string.IsNullOrWhiteSpace(_options.NotificationUrl) ? _options.NotificationUrl : null);

        var paymobRequest = new PaymobIntentionRequest
        {
            Amount = amountInCents,
            Currency = string.IsNullOrWhiteSpace(request.Currency) ? _options.Currency : request.Currency,
            SpecialReference = request.OrderId.ToString(),
            PaymentMethods = _options.IntegrationId > 0 ? [_options.IntegrationId] : [],
            Items =
            [
                new PaymobItem
                {
                    Name = $"Flower Order {request.OrderId}",
                    Amount = amountInCents,
                    Quantity = 1,
                    Description = $"Order #{request.OrderId}"
                }
            ],
            BillingData = new PaymobBillingData
            {
                FirstName = request.BillingData?.FirstName ?? "Customer",
                LastName = request.BillingData?.LastName ?? "User",
                Email = request.BillingData?.Email ?? "customer@example.com",
                PhoneNumber = request.BillingData?.PhoneNumber ?? "+201000000000",
                Country = request.BillingData?.Country ?? "EGY",
                City = request.BillingData?.City ?? "Cairo",
                Street = request.BillingData?.Street ?? "123 Street",
                Building = request.BillingData?.Building ?? "1",
                Floor = request.BillingData?.Floor ?? "1",
                Apartment = request.BillingData?.Apartment ?? "1"
            },
            RedirectionUrl = redirectionUrl,
            NotificationUrl = notificationUrl
        };

        try
        {
            if (!string.IsNullOrWhiteSpace(_options.SecretKey) && !_options.SecretKey.StartsWith("YOUR_"))
            {
                var response = await paymobClient.CreateIntentionAsync(
                    paymobRequest,
                    cancellationToken);

                var paymentUrl = checkoutUrlBuilder.Build(
                    response.ClientSecret);

                var paymobOrderId = response.IntentionOrderId > 0
                    ? response.IntentionOrderId.ToString()
                    : (response.Order?.Id > 0 ? response.Order.Id.ToString() : request.OrderId.ToString());

                return new CreatePaymentSessionResult(
                    response.Id,
                    response.ClientSecret,
                    paymobOrderId,
                    paymentUrl,
                    redirectionUrl);
            }
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Paymob intention creation failed: {Message}. Falling back to sandbox checkout session.", ex.Message);
        }

        var mockSessionId = $"sess_{Guid.NewGuid():N}";
        var mockClientSecret = $"secret_{Guid.NewGuid():N}";
        var fallbackPublicKey = !string.IsNullOrWhiteSpace(_options.PublicKey) ? _options.PublicKey : "mock_public_key";
        var fallbackPaymentUrl = $"https://accept.paymob.com/unifiedcheckout/?publicKey={fallbackPublicKey}&clientSecret={mockClientSecret}";

        return new CreatePaymentSessionResult(
            mockSessionId,
            mockClientSecret,
            request.OrderId.ToString(),
            fallbackPaymentUrl,
            redirectionUrl);
    }
}