namespace Cart_Service.Features.GetCart.ViewModels
{
    public record GetCartItemResponse(
        Guid Id,
        Guid ProductId,
        string ProductName,
        string ProductImageUrl,
        decimal UnitPrice,
        int Quantity,
        decimal LineSubtotal,
        bool InStock,
        int? AvailableStock,
        bool PriceChanged);
}
