using Blocks.Contracts.Common;
using Blocks.Domain.Errors;
using MediatR;
using Order___Fulfillment_Service.Services;

namespace Order___Fulfillment_Service.Features.Checkout.GetCheckoutDetails;

public sealed class GetCheckoutDetailsQueryHandler : IRequestHandler<GetCheckoutDetailsQuery, Result<CheckoutDetailsResponse>>
{
    private readonly ICartServiceClient _cartService;
    private readonly IAddressServiceClient _addressService;

    public GetCheckoutDetailsQueryHandler(
        ICartServiceClient cartService,
        IAddressServiceClient addressService)
    {
        _cartService = cartService;
        _addressService = addressService;
    }

    public async Task<Result<CheckoutDetailsResponse>> Handle(
        GetCheckoutDetailsQuery request,
        CancellationToken cancellationToken)
    {
        // 1. Concurrent parallel fetch for user cart and user addresses (Task.WhenAll)
        var cartTask = request.CartId.HasValue && request.CartId.Value != Guid.Empty
            ? _cartService.GetCartByIdAsync(request.CartId.Value, request.BearerToken, cancellationToken)
            : _cartService.GetUserCartAsync(request.BearerToken, cancellationToken);
        var addressesTask = _addressService.GetUserAddressesAsync(request.BearerToken, cancellationToken);

        await Task.WhenAll(cartTask, addressesTask);

        var cart = await cartTask ?? await _cartService.GetUserCartAsync(request.BearerToken, cancellationToken);
        var addresses = await addressesTask;

        // 2. Validate Cart
        if (cart is null || cart.Items.Count == 0)
        {
            return Result.Failure<CheckoutDetailsResponse>(
                Error.Validation("Your cart is empty. Please add items to cart before proceeding to checkout."));
        }

        var subtotal = cart.Subtotal > 0
            ? cart.Subtotal
            : cart.Items.Sum(i => i.UnitPrice * i.Quantity);

        // 3. Resolve default address & store coverage
        var defaultAddress = addresses.FirstOrDefault(a => a.IsDefault) ?? addresses.FirstOrDefault();
        decimal? deliveryFee = null;
        DateTime? estimatedDeliveryAt = null;
        bool isServiceable = false;

        if (defaultAddress is not null)
        {
            var coverage = await _addressService.GetNearestCoveringStoreAsync(
                defaultAddress.Latitude,
                defaultAddress.Longitude,
                cancellationToken);

            if (coverage is not null && coverage.IsServiceable)
            {
                isServiceable = true;
                deliveryFee = coverage.DeliveryFee;
                estimatedDeliveryAt = DateTime.UtcNow.AddMinutes(coverage.EstimatedDeliveryMinutes);
            }
        }

        var total = subtotal + (deliveryFee ?? 0m);
        var cartId = cart.Id != Guid.Empty ? cart.Id : (request.CartId ?? Guid.NewGuid());

        var response = new CheckoutDetailsResponse(
            CartId: cartId,
            AddressId: defaultAddress?.Id,
            IsServiceable: isServiceable,
            Subtotal: subtotal,
            DeliveryFee: deliveryFee,
            Total: total,
            EstimatedDeliveryAt: estimatedDeliveryAt,
            PaymentMethods: CheckoutDetailsResponse.DefaultPaymentMethods,
            IsGift: false,
            GiftRecipientName: null,
            GiftRecipientPhone: null
        );

        return Result.Success(response);
    }
}
