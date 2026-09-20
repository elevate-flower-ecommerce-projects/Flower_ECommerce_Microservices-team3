namespace Cart_Service.Features.Cart.DTOs;

// =========================================================================================================
// [TEMPORARY BUILD] Placeholder models for Cart Summary until intern completes final implementation.
// =========================================================================================================

public record CartSummaryDto(
    IReadOnlyList<CartItemSummaryDto> Items,
    decimal Subtotal,
    decimal DeliveryFee,
    decimal Total,
    bool HasChanges
);

public record CartItemSummaryDto(
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
