namespace Order___Fulfillment_Service.Features.Orders.GetOrderById.DTOs
{
    public sealed record OrderItemDto(
    Guid ProductId,
    string ProductName,
    int Quantity,
    decimal UnitPrice,
    string? ThumbnailUrl);
}
