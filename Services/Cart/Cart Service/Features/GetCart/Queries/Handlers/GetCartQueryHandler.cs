using Blocks.Contracts.Common;
using Blocks.Contracts.Interfaces;
using Cart_Service.Features.GetCart.ViewModels;
using Cart_Service.Services;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Cart_Service.Features.GetCart.Queries.Handlers
{
    public sealed class GetCartQueryHandler : IRequestHandler<GetCartQuery, Result<GetCartResponse>>
    {
        private readonly IGenericRepository<Entities.Cart> _cartRepository;
        private readonly ICatalogServiceClient _catalogService;

        public GetCartQueryHandler(
            IGenericRepository<Entities.Cart> cartRepository,
            ICatalogServiceClient catalogService)
        {
            _cartRepository = cartRepository;
            _catalogService = catalogService;
        }

        public async Task<Result<GetCartResponse>> Handle(GetCartQuery request, CancellationToken cancellationToken)
        {
            var cartProjection = await _cartRepository.GetQueryable()
                .AsNoTracking()
                .Where(c => c.CustomerId == request.CustomerId)
                .Select(c => new CartProjection
                {
                    Id = c.Id,
                    CustomerId = c.CustomerId,
                    Items = c.Items.Select(i => new CartItemProjection
                    {
                        Id = i.Id,
                        ProductId = i.ProductId,
                        Quantity = i.Quantity,
                        StoredUnitPrice = i.UnitPrice
                    }).ToList()
                })
                .FirstOrDefaultAsync(cancellationToken);

            if (cartProjection is null)
            {
                return Result.Success(EmptyCart(request.CustomerId));
            }

            if (cartProjection.Items.Count == 0)
            {
                return Result.Success(EmptyCart(request.CustomerId, cartProjection.Id));
            }

            var productIds = cartProjection.Items.Select(i => i.ProductId);
            var catalogProducts = await _catalogService.GetProductsByIdsAsync(productIds, cancellationToken);

            var hasChanges = false;
            var responseItems = new List<GetCartItemResponse>(cartProjection.Items.Count);

            foreach (var item in cartProjection.Items)
            {
                catalogProducts.TryGetValue(item.ProductId, out var product);

                var currentPrice = product?.Price ?? item.StoredUnitPrice;
                var priceChanged = product is not null && product.Price != item.StoredUnitPrice;
                var inStock = product?.InStock ?? true;

                if (priceChanged || !inStock)
                    hasChanges = true;

                responseItems.Add(new GetCartItemResponse(
                    Id: item.Id,
                    ProductId: item.ProductId,
                    ProductName: product?.Name ?? "Unknown Product",
                    ProductImageUrl: product?.ImageUrl ?? string.Empty,
                    UnitPrice: currentPrice,
                    Quantity: item.Quantity,
                    LineSubtotal: currentPrice * item.Quantity,
                    InStock: inStock,
                    AvailableStock: null,
                    PriceChanged: priceChanged));
            }

            var subtotal = responseItems.Sum(i => i.LineSubtotal);

            var response = new GetCartResponse(
                Id: cartProjection.Id,
                CustomerId: cartProjection.CustomerId,
                Items: responseItems,
                Subtotal: subtotal,
                DeliveryFee: null,
                Total: subtotal,
                HasChanges: hasChanges);

            return Result.Success(response);
        }

        private static GetCartResponse EmptyCart(Guid customerId, Guid? cartId = null) =>
            new(
                Id: cartId ?? Guid.Empty,
                CustomerId: customerId,
                Items: Array.Empty<GetCartItemResponse>(),
                Subtotal: 0m,
                DeliveryFee: null,
                Total: 0m,
                HasChanges: false);

        private sealed class CartProjection
        {
            public Guid Id { get; init; }
            public Guid CustomerId { get; init; }
            public List<CartItemProjection> Items { get; init; } = [];
        }

        private sealed class CartItemProjection
        {
            public Guid Id { get; init; }
            public Guid ProductId { get; init; }
            public int Quantity { get; init; }
            public decimal StoredUnitPrice { get; init; }
        }
    }
}
