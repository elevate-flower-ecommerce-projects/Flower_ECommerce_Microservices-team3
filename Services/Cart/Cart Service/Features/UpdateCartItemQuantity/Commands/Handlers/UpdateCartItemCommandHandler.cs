using Blocks.Contracts.Common;
using Blocks.Contracts.Interfaces;
using Blocks.Domain.Errors;
using Cart_Service.Features.Cart.ViewModels;
using Cart_Service.Entities;
using Cart_Service.Features.UpdateCartItemQuantity.ViewModels;
using Cart_Service.Persistence;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Cart_Service.Features.UpdateCartItemQuantity.Commands.Handlers
{
    public class UpdateCartItemCommandHandler : IRequestHandler<UpdateCartItemCommand, Result<CartResponse>>
    {
        private readonly IGenericRepository<Entities.Cart> _cartRepository;
        private readonly IUnitOfWork _unitOfWork;

        public UpdateCartItemCommandHandler(
            IGenericRepository<Entities.Cart> cartRepository,
            IUnitOfWork unitOfWork)
        {
            _cartRepository = cartRepository;
            _unitOfWork = unitOfWork;
        }

        public async Task<Result<CartResponse>> Handle(UpdateCartItemCommand request, CancellationToken cancellationToken)
        {
            var cart = await _cartRepository.GetQueryable()
                .Include(c => c.Items)
                .FirstOrDefaultAsync(c => c.CustomerId == request.CustomerId, cancellationToken);

            if (cart is null)
            {
                return Result.Failure<CartResponse>(Error.NotFound("Cart not found for this customer."));
            }

            var cartItem = cart.FindItem(request.ProductId);
            if (cartItem is null)
            {
                return Result.Failure<CartResponse>(Error.NotFound("Product is not in the cart."));
            }

            if (request.Quantity == 0)
            {
                cart.RemoveItem(request.ProductId);
            }
            else
            {
                int mockedAvailableStock = 10;

                if (request.Quantity > mockedAvailableStock)
                {
                    return Result.Failure<CartResponse>(Error.Conflict($"Only {mockedAvailableStock} left in stock."));
                }

                cartItem.UpdateQuantity(request.Quantity);
                cart.RecalculateTotals();
            }

            await _unitOfWork.SaveChangesAsync(cancellationToken);

            var responseItems = cart.Items.Select(i => new CartItemResponse(
                i.Id,
                i.ProductId,
                i.Quantity,
                i.UnitPrice,
                i.LineTotal
            )).ToList();

            var response = new CartResponse(
                cart.Id,
                cart.CustomerId,
                cart.Subtotal,
                cart.Total,
                responseItems);

            return Result.Success(response);
        }
    }
}
