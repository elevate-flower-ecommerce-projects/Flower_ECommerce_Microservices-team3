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
        var cartTask = _cartService.GetUserCartAsync(request.BearerToken, cancellationToken);
        var addressesTask = _addressService.GetUserAddressesAsync(request.BearerToken, cancellationToken);

        await Task.WhenAll(cartTask, addressesTask);

        var cart = await cartTask;
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
        decimal deliveryFee = 0.00m;
        string? estimatedDeliveryAt = null;

        if (defaultAddress is not null)
        {
            var coverage = await _addressService.GetNearestCoveringStoreAsync(
                defaultAddress.Latitude,
                defaultAddress.Longitude,
                cancellationToken);

            if (coverage is not null && coverage.IsServiceable)
            {
                deliveryFee = coverage.DeliveryFee;
                estimatedDeliveryAt = DateTime.UtcNow.AddMinutes(coverage.EstimatedDeliveryMinutes).ToString("g");
            }
        }

        var total = subtotal + deliveryFee;

        var response = new CheckoutDetailsResponse(
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
