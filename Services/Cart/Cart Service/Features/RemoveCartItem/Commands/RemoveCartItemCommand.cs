using Blocks.Contracts.Common;
using Cart_Service.Features.AddToCartItem;
using MediatR;

namespace Cart_Service.Features.RemoveCartItem.Commands;

public record RemoveCartItemCommand(
    Guid CustomerId,
    Guid CartItemId,
    string Language
) : IRequest<Result<CartSummaryResponse>>;
