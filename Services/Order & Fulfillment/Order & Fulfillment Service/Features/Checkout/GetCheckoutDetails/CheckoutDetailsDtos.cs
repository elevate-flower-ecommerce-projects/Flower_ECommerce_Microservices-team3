using System.Text.Json.Serialization;

namespace Order___Fulfillment_Service.Features.Checkout.GetCheckoutDetails;

public sealed record CheckoutDetailsResponse(
    decimal Subtotal,
    decimal DeliveryFee,
    decimal Total,
    string? EstimatedDeliveryAt,
    IReadOnlyList<PaymentMethodOptionDto> PaymentMethods,
    bool IsGift = false,
    string? GiftRecipientName = null,
    string? GiftRecipientPhone = null
)
{
    public static readonly IReadOnlyList<PaymentMethodOptionDto> DefaultPaymentMethods =
    [
        new PaymentMethodOptionDto("COD"),
        new PaymentMethodOptionDto("Card", ["Stripe"])
    ];
}

public sealed record PaymentMethodOptionDto(
    string Method,
    [property: JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    IReadOnlyList<string>? Gateways = null
);
