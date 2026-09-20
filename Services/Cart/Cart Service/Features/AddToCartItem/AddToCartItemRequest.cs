namespace Cart_Service.Features.AddToCartItem;

public record AddToCartItemRequest(
    Guid ProductId,
    int Quantity
);
