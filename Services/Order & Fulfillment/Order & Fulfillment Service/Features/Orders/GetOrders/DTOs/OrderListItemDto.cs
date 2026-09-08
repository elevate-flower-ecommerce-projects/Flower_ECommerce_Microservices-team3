using Order___Fulfillment_Service.Entities.Enums;

namespace Order___Fulfillment_Service.Features.Orders.GetOrders.DTOs
{
    public sealed record OrderListItemDto(
    Guid Id,
    OrderStatus Status,
    int ItemCount,
    string? ThumbnailUrl,
    decimal Total,
    DateTime EstimatedDeliveryAt,
    DateTime CreatedAt);
}
