using Blocks.Contracts.Common;
using Blocks.Contracts.Interfaces;
using Blocks.Domain.Errors;
using Cart_Service.Entities;
using Cart_Service.Features.Cart.DTOs;
using Cart_Service.Persistence;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Cart_Service.Features.AddToCartItem.Commands.Handlers;

public class AddToCartItemCommandHandler(
    IGenericRepository<Entities.Cart> cartRepository,
    IUnitOfWork unitOfWork)
    : IRequestHandler<AddToCartItemCommand, Result<CartSummaryDto>>
{
    public async Task<Result<CartSummaryDto>> Handle(AddToCartItemCommand request, CancellationToken cancellationToken)
    {
        const int availableStock = 50;
        const decimal unitPrice = 150m;

        // Load customer's cart
        var cart = await cartRepository.GetQueryable()
            .Include(c => c.Items)
            .FirstOrDefaultAsync(c => c.CustomerId == request.CustomerId, cancellationToken);

        var isNewCart = false;
        if (cart is null)
        {
            cart = Entities.Cart.Create(request.CustomerId);
            isNewCart = true;
        }

        var existingItem = cart.FindItem(request.ProductId);
        var currentQuantity = existingItem?.Quantity ?? 0;
        var targetQuantity = currentQuantity + request.Quantity;

        if (targetQuantity > availableStock)
        {
            return Result.Failure<CartSummaryDto>(
                Error.Validation($"Product stock exceeded. Requested {targetQuantity}, available {availableStock}.", "quantity"));
        }

        cart.AddItem(request.ProductId, request.Quantity, unitPrice);

        if (isNewCart)
        {
            await cartRepository.AddAsync(cart, cancellationToken);
        }

        await unitOfWork.SaveChangesAsync(cancellationToken);

        // Populate response
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
