using Blocks.Contracts.Common;
using Blocks.Contracts.Interfaces;
using Blocks.Domain.Errors;
using Cart_Service.Entities;
using Cart_Service.Features.Cart.DTOs;
using Cart_Service.Persistence;
using Cart_Service.Services;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Cart_Service.Features.AddToCartItem.Commands.Handlers;

public class AddToCartItemCommandHandler : IRequestHandler<AddToCartItemCommand, Result<CartSummaryDto>>
{
    private readonly IGenericRepository<Entities.Cart> _cartRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ICatalogServiceClient _catalogService;

    public AddToCartItemCommandHandler(
        IGenericRepository<Entities.Cart> cartRepository,
        IUnitOfWork unitOfWork,
        ICatalogServiceClient catalogService)
    {
        _cartRepository = cartRepository;
        _unitOfWork = unitOfWork;
        _catalogService = catalogService;
    }

    public async Task<Result<CartSummaryDto>> Handle(AddToCartItemCommand request, CancellationToken cancellationToken)
    {
        // 1. Fetch live product info from catalog service
        var product = await _catalogService.GetProductByIdAsync(request.ProductId, cancellationToken);
        if (product is not null && !product.InStock)
        {
            return Result.Failure<CartSummaryDto>(
                Error.Conflict("Product is currently out of stock."));
        }

        var unitPrice = product?.Price > 0 ? product.Price : 150m;
        const int availableStock = 50;

        // 2. Load customer's cart
        var cart = await _cartRepository.GetQueryable()
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
            await _cartRepository.AddAsync(cart, cancellationToken);
        }

        await _unitOfWork.SaveChangesAsync(cancellationToken);

        // 3. Populate response
        var isArabic = request.Language.StartsWith("ar", StringComparison.OrdinalIgnoreCase);

        var productIds = cart.Items.Select(i => i.ProductId);
        var catalogProducts = await _catalogService.GetProductsByIdsAsync(productIds, cancellationToken);

        var responseItems = cart.Items.Select(i =>
        {
            catalogProducts.TryGetValue(i.ProductId, out var catProd);
            return new CartItemSummaryDto(
                Id: i.Id,
                ProductId: i.ProductId,
                ProductName: catProd?.Name ?? (isArabic ? "باقة زهور مميزة" : "Fresh Flower Arrangement"),
                ProductImageUrl: !string.IsNullOrEmpty(catProd?.ImageUrl) ? catProd.ImageUrl : "categories/tulip_flower.png",
                UnitPrice: i.UnitPrice,
                Quantity: i.Quantity,
                LineSubtotal: i.LineTotal,
                InStock: catProd?.InStock ?? true,
                AvailableStock: availableStock
            );
        }).ToList();

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
