using Blocks.Contracts.Common;
using Cart_Service.Features.Cart.ViewModels;
using MediatR;

namespace Cart_Service.Features.UpdateCartItemQuantity.Commands
{
    public record UpdateCartItemCommand(
    Guid CustomerId,
    Guid ProductId,
    int Quantity) : IRequest<Result<CartResponse>>;
}
