using Blocks.Contracts.Common;
using Blocks.Contracts.Interfaces;
using CartEntity = Cart_Service.Entities.Cart;
using Blocks.Domain.Errors;
using Cart_Service.Abstractions;
using Cart_Service.Infrastructure.Interfaces;
using MediatR;

namespace Cart_Service.Features.AddToCartItem;

public sealed class AddToCartItemCommandHandler(
    IUnitOfWork unitOfWork,
    ICartRepository cartRepository,
    IProductCatalogClient productCatalogClient)
    : IRequestHandler<AddToCartItemCommand, Result<CartSummaryResponse>>
{
    public async Task<Result<CartSummaryResponse>> Handle(
        AddToCartItemCommand request,
        CancellationToken cancellationToken)
    {
        // 1. Get product and current stock
        var productResult =
            await productCatalogClient.GetProductAsync(
                request.ProductId,
                request.Language,
                cancellationToken);

        if (productResult.IsFailure)
            return productResult.Error!;

        var product = productResult.Value;

        // 2. Validate product stock
        if (product.AvailableStock <= 0)
        {
            return Error.BadRequest(
                "Product is out of stock.");
        }

        // 3. Get customer's cart
        var cart =
            await cartRepository.GetByCustomerIdAsync(
                request.CustomerId,
                cancellationToken);

        // 4. Create cart if it doesn't exist
        if (cart is null)
        {
            cart = CartEntity.Create(request.CustomerId);

            await cartRepository.AddAsync(
                cart,
                cancellationToken);
        }

        // 5. Check existing item
        var existingItem =
            cart.FindItem(request.ProductId);

        var finalQuantity =
            (existingItem?.Quantity ?? 0) + request.Quantity;

        // 6. Validate final quantity against stock
        if (finalQuantity > product.AvailableStock)
        {
            return Error.BadRequest(
                $"Only {product.AvailableStock} items are available.");
        }

        // 7. Add item or increase quantity
        cart.AddItem(
            product.ProductId,
            request.Quantity,
            product.UnitPrice);

        // 8. Get latest product information
        //    for all cart items before saving.
        var productIds = cart.Items
            .Select(x => x.ProductId)
            .Distinct()
            .ToList();

        var productsResult =
            await productCatalogClient.GetProductsAsync(
                productIds,
                request.Language,
                cancellationToken);

        if (productsResult.IsFailure)
            return productsResult.Error!;

        var products =
            productsResult.Value
                .ToDictionary(x => x.ProductId);

        // 9. Build response
        var items = cart.Items
            .Select(item =>
            {
                if (!products.TryGetValue(
                        item.ProductId,
                        out var currentProduct))
                {
                    return new CartItemSummaryResponse(
                        item.Id,
                        item.ProductId,
                        string.Empty,
                        string.Empty,
                        item.UnitPrice,
                        item.Quantity,
                        item.LineTotal,
                        false,
                        0);
                }

                return new CartItemSummaryResponse(
                    item.Id,
                    item.ProductId,
                    currentProduct.ProductName,
                    currentProduct.ProductImageUrl,
                    item.UnitPrice,
                    item.Quantity,
                    item.LineTotal,
                    currentProduct.AvailableStock > 0,
                    currentProduct.AvailableStock);
            })
            .ToList();

        // 10. Save cart changes
        await unitOfWork.SaveChangesAsync(
            cancellationToken);

        // 11. Return updated cart summary
        return new CartSummaryResponse(
            Items: items,
            Subtotal: cart.Subtotal,
            DeliveryFee: 0,
            Total: cart.Total,
            HasChanges: true);
    }
}