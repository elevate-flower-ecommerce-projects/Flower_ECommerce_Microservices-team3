namespace Cart_Service.Features.AddToCartItem.DTOs;

// =========================================================================================================
// [TEMPORARY BUILD] Temporary placeholder request DTO for AddItemToCart (SCRUM-11 / SCRUM-87 / SCRUM-88).
// =========================================================================================================

public record AddToCartItemRequestDto(
    Guid ProductId,
    int Quantity
);
