namespace Cart_Service.Features.AddToCartItem;


public record CartSummaryResponse(
    IReadOnlyList<CartItemSummaryResponse> Items,
    decimal Subtotal,
    decimal DeliveryFee,
    decimal Total,
    bool HasChanges
);

public record CartItemSummaryResponse(
    Guid Id,
    Guid ProductId,
    string ProductName,
    string ProductImageUrl,
    decimal UnitPrice,
    int Quantity,
    decimal LineSubtotal,
    bool InStock,
    int AvailableStock
);
