using System.Text.Json.Serialization;

namespace Order___Fulfillment_Service.Features.Checkout.GetCheckoutDetails;

public sealed record CheckoutDetailsResponse(
    Guid CartId,
    Guid? AddressId,
    bool IsServiceable,
    decimal Subtotal,
    decimal? DeliveryFee,
    decimal Total,
    DateTime? EstimatedDeliveryAt,
    IReadOnlyList<PaymentMethodOptionDto> PaymentMethods,
    bool IsGift = false,
    string? GiftRecipientName = null,
    string? GiftRecipientPhone = null
)
{
    public static readonly IReadOnlyList<PaymentMethodOptionDto> DefaultPaymentMethods =
    [
        new PaymentMethodOptionDto("COD"),
        new PaymentMethodOptionDto("Card", ["Paymob"])
    ];
}

public sealed record PaymentMethodOptionDto(
    string Method,
    [property: JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    IReadOnlyList<string>? Gateways = null
);
