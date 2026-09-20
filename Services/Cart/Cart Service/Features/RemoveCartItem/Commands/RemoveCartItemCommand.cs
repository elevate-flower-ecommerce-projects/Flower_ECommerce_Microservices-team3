using Blocks.Contracts.Common;
using Cart_Service.Features.Cart.DTOs;
using MediatR;

namespace Cart_Service.Features.RemoveCartItem.Commands;

// =========================================================================================================
// [TEMPORARY BUILD] Temporary placeholder command for RemoveCartItem (SCRUM-25 / SCRUM-97) until intern finishes.
// =========================================================================================================

public record RemoveCartItemCommand(
    Guid CustomerId,
    Guid CartItemId,
    string Language
) : IRequest<Result<CartSummaryDto>>;
