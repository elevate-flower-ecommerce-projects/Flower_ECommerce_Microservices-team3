using Blocks.Contracts.Payment;
using Microsoft.Extensions.Options;
using Payment_Service.Application.Abstractions;
using Payment_Service.Application.Models;
using Payment_Service.Infrastructure.Paymob;

namespace Payment_Service.Infrastructure;

public sealed class PaymentGateway(
    IPaymobClient paymobClient,
    IPaymobCheckoutUrlBuilder checkoutUrlBuilder,
    IOptions<PaymobOptions> options)
    : IPaymentGateway
{
    private readonly PaymobOptions _options = options.Value;

    public async Task<CreatePaymentSessionResult> CreateCheckoutSessionAsync(
        CreatePaymentSessionRequest request,
        CancellationToken cancellationToken = default)
    {
        var amountInCents = Convert.ToInt32(request.Amount * 100);

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
                FirstName = request.BillingData.FirstName,
                LastName = request.BillingData.LastName,
                Email = request.BillingData.Email,
                PhoneNumber = request.BillingData.PhoneNumber,
                Country = request.BillingData.Country,
                City = request.BillingData.City,
                Street = request.BillingData.Street,
                Building = request.BillingData.Building,
                Floor = request.BillingData.Floor,
                Apartment = request.BillingData.Apartment
            }
        };

        var response = await paymobClient.CreateIntentionAsync(
            paymobRequest,
            cancellationToken);

        var paymentUrl = checkoutUrlBuilder.Build(
            response.ClientSecret);

        return new CreatePaymentSessionResult(
            response.Id,
            response.ClientSecret,
            response.Order?.Id.ToString() ?? string.Empty,
            paymentUrl);
    }
}