using Blocks.Contracts.Common;
using MediatR;

namespace Cart_Service.Features.AddToCartItem;

public sealed record AddToCartItemCommand(
    Guid CustomerId,
    Guid ProductId,
    int Quantity,
    string Language
) : IRequest<Result<CartSummaryResponse>>;
