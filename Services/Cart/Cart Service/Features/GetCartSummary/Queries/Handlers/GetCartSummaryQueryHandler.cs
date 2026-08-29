using Blocks.Contracts.Common;
using Blocks.Contracts.Interfaces;
using Cart_Service.Entities;
using Cart_Service.Features.Cart.DTOs;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Cart_Service.Features.GetCartSummary.Queries.Handlers;

// =========================================================================================================
// [TEMPORARY BUILD] Temporary placeholder query handler for GetCartSummary (SCRUM-29 / SCRUM-107) until intern finishes.
// =========================================================================================================

public class GetCartSummaryQueryHandler : IRequestHandler<GetCartSummaryQuery, Result<CartSummaryDto>>
{
    private readonly IGenericRepository<Entities.Cart> _cartRepository;

    public GetCartSummaryQueryHandler(IGenericRepository<Entities.Cart> cartRepository)
    {
        _cartRepository = cartRepository;
    }

    public async Task<Result<CartSummaryDto>> Handle(GetCartSummaryQuery request, CancellationToken cancellationToken)
    {
        var cart = await _cartRepository.GetQueryable()
            .AsNoTracking()
            .Include(c => c.Items)
            .FirstOrDefaultAsync(c => c.CustomerId == request.CustomerId, cancellationToken);

        if (cart is null || !cart.Items.Any())
        {
            return Result.Success(new CartSummaryDto(
                Items: Array.Empty<CartItemSummaryDto>(),
                Subtotal: 0m,
                DeliveryFee: 0m,
                Total: 0m,
                HasChanges: false
            ));
        }

        var isArabic = request.Language.StartsWith("ar", StringComparison.OrdinalIgnoreCase);

        var items = cart.Items.Select(i => new CartItemSummaryDto(
            Id: i.Id,
            ProductId: i.ProductId,
            ProductName: isArabic ? "باقة زهور مميزة" : "Fresh Flower Arrangement",
            ProductImageUrl: "categories/tulip_flower.png",
            UnitPrice: i.UnitPrice,
            Quantity: i.Quantity,
            LineSubtotal: i.LineTotal,
            InStock: true,
            AvailableStock: 50
        )).ToList();

        var subtotal = cart.Items.Sum(i => i.LineTotal);
        decimal deliveryFee = 0m;
        var total = subtotal + deliveryFee;

        return Result.Success(new CartSummaryDto(
            Items: items,
            Subtotal: subtotal,
            DeliveryFee: deliveryFee,
            Total: total,
            HasChanges: false
        ));
    }
}
