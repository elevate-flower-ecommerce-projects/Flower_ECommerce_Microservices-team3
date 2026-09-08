using Order___Fulfillment_Service.Entities.Enums;

namespace Order___Fulfillment_Service.Features.Orders.GetOrderById.DTOs
{
    public sealed record OrderDetailDto(
    Guid Id,
    OrderStatus Status,
    PaymentMethod PaymentMethod,
    PaymentGateway? PaymentGateway,
    List<OrderItemDto> Items,
    OrderAddressDto Address,
    decimal Subtotal,
    decimal DeliveryFee,
    decimal Total,
    bool IsGift,
    string? GiftRecipientName,
    string? GiftRecipientPhone,
    DateTime EstimatedDeliveryAt,
    DateTime CreatedAt);
}
