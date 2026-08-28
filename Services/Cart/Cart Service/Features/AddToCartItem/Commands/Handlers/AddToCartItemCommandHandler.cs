using Blocks.Contracts.Common;
using Blocks.Contracts.Interfaces;
using Blocks.Domain.Errors;
using Cart_Service.Entities;
using Cart_Service.Features.Cart.DTOs;
using Cart_Service.Persistence;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Cart_Service.Features.AddToCartItem.Commands.Handlers;

// =========================================================================================================
// [TEMPORARY BUILD] Temporary placeholder handler for AddItemToCart (SCRUM-11 / SCRUM-87 / SCRUM-88).
// =========================================================================================================

public class AddToCartItemCommandHandler : IRequestHandler<AddToCartItemCommand, Result<CartSummaryDto>>
{
    private readonly IGenericRepository<Entities.Cart> _cartRepository;
    private readonly IUnitOfWork _unitOfWork;

    public AddToCartItemCommandHandler(
        IGenericRepository<Entities.Cart> cartRepository,
        IUnitOfWork unitOfWork)
    {
        _cartRepository = cartRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result<CartSummaryDto>> Handle(AddToCartItemCommand request, CancellationToken cancellationToken)
    {
        var cart = await _cartRepository.GetQueryable()
            .Include(c => c.Items)
            .FirstOrDefaultAsync(c => c.CustomerId == request.CustomerId, cancellationToken);

        if (cart is null)
        {
            cart = Entities.Cart.Create(request.CustomerId);
            await _cartRepository.AddAsync(cart, cancellationToken);
        }

        const int availableStock = 50;
        var existingItem = cart.FindItem(request.ProductId);
        var currentQuantity = existingItem?.Quantity ?? 0;
        var targetQuantity = currentQuantity + request.Quantity;

        if (targetQuantity > availableStock)
        {
            return Result.Failure<CartSummaryDto>(
                Error.Validation($"Product stock exceeded. Requested {targetQuantity}, available {availableStock}.", "quantity"));
        }

        const decimal defaultUnitPrice = 150m;
        cart.AddItem(request.ProductId, request.Quantity, defaultUnitPrice);

        _cartRepository.Update(cart);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        var isArabic = request.Language.StartsWith("ar", StringComparison.OrdinalIgnoreCase);

        var responseItems = cart.Items.Select(i => new CartItemSummaryDto(
            Id: i.Id,
            ProductId: i.ProductId,
            ProductName: isArabic ? "باقة زهور مميزة" : "Fresh Flower Arrangement",
            ProductImageUrl: "categories/tulip_flower.png",
            UnitPrice: i.UnitPrice,
            Quantity: i.Quantity,
            LineSubtotal: i.LineTotal,
            InStock: true,
            AvailableStock: availableStock
        )).ToList();

        var subtotal = cart.Items.Sum(i => i.LineTotal);
        decimal deliveryFee = 0m;
        var total = subtotal + deliveryFee;

        return Result.Success(new CartSummaryDto(
            Items: responseItems,
            Subtotal: subtotal,
            DeliveryFee: deliveryFee,
            Total: total,
            HasChanges: false
        ));
    }
}
