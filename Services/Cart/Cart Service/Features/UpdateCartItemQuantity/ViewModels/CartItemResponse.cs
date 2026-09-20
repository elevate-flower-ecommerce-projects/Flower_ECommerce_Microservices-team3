namespace Cart_Service.Features.UpdateCartItemQuantity.ViewModels
{
    public record CartItemResponse(
    Guid Id,
    Guid ProductId,
    int Quantity,
    decimal UnitPrice,
    decimal LineTotal);
}
