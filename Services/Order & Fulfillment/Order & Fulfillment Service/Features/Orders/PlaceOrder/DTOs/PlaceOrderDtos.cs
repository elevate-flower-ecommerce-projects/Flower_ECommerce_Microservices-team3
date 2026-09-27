using System.Text.Json.Serialization;
using Order___Fulfillment_Service.Entities.Enums;

namespace Order___Fulfillment_Service.Features.Orders.PlaceOrder.DTOs;

public sealed record PlaceOrderRequest(
    Guid? CartId = null,
    Guid? AddressId = null,
    PaymentMethod PaymentMethod = PaymentMethod.COD,
    string? PaymentGateway = null,
    bool IsGift = false,
    GiftRecipientDto? GiftRecipient = null,
    string? Notes = null,
    string? CustomerPhone = null,
    string? GiftRecipientName = null,
    string? GiftRecipientPhone = null
);

public sealed record GiftRecipientDto(
    string RecipientName,
    string RecipientPhone
);

public sealed record PlaceOrderCardResult(
    Guid OrderId,
    string Status,
    string Gateway,
    string SessionId,
    string SessionUrl,
    string SuccessUrl,
    string CancelUrl,
    DateTime ExpiresAt,
    decimal Amount,
    string Currency,
    DateTime EstimatedDeliveryAt
);
