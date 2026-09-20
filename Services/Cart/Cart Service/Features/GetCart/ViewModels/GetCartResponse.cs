namespace Cart_Service.Features.GetCart.ViewModels
{
    public record GetCartResponse(
        Guid Id,
        Guid CustomerId,
        IReadOnlyCollection<GetCartItemResponse> Items,
        decimal Subtotal,
        decimal? DeliveryFee,
        decimal Total,
        bool HasChanges);
}
