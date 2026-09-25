using System.Text.Json.Serialization;
using Blocks.Contracts.Payment;
using Order___Fulfillment_Service.Entities.Enums;

namespace Order___Fulfillment_Service.Features.Orders.PlaceOrder.DTOs;

public sealed record PlaceOrderRequest(
    Guid? AddressId = null,
    PaymentMethod PaymentMethod = PaymentMethod.COD,
    bool IsGift = false,
    string? GiftRecipientName = null,
    string? GiftRecipientPhone = null,
    string? Notes = null
);

public sealed record PlaceOrderResponse(
    Guid OrderId,
    OrderStatus Status,
    PaymentMethod PaymentMethod,
    decimal Subtotal,
    decimal DeliveryFee,
    decimal Total,
    DateTime EstimatedDeliveryAt,
    string? PaymentUrl = null,
    string? SessionId = null
);
