using Cart_Service.Features.UpdateCartItemQuantity.ViewModels;

namespace Cart_Service.Features.Cart.ViewModels
{
    public record CartResponse(
    Guid Id,
    Guid CustomerId,
    decimal Subtotal,
    decimal Total,
    IReadOnlyCollection<CartItemResponse> Items);
}