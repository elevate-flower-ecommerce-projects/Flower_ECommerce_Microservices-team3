using Blocks.Contracts.Common;
using Cart_Service.Features.Cart.DTOs;
using MediatR;

namespace Cart_Service.Features.AddToCartItem.Commands;

// =========================================================================================================
// [TEMPORARY BUILD] Temporary placeholder command for AddItemToCart (SCRUM-11 / SCRUM-87 / SCRUM-88).
// =========================================================================================================

public record AddToCartItemCommand(
    Guid CustomerId,
    Guid ProductId,
    int Quantity,
    string Language
) : IRequest<Result<CartSummaryDto>>;
