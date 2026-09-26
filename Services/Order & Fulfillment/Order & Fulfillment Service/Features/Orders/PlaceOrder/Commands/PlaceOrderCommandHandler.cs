using Blocks.Contracts.Common;
using Blocks.Contracts.Payment;
using Blocks.Domain.Errors;
using MediatR;
using Microsoft.Extensions.Logging;
using Order___Fulfillment_Service.Entities;
using Order___Fulfillment_Service.Entities.Enums;
using Order___Fulfillment_Service.Features.Orders.PlaceOrder.DTOs;
using Order___Fulfillment_Service.Persistence;
using Order___Fulfillment_Service.Services;

namespace Order___Fulfillment_Service.Features.Orders.PlaceOrder.Commands;

public sealed class PlaceOrderCommandHandler(
    FlowersOrderDbContext dbContext,
    ICartServiceClient cartService,
    IAddressServiceClient addressService,
    IPaymentServiceClient paymentService,
    IUnitOfWork unitOfWork,
    ILogger<PlaceOrderCommandHandler> logger)
    : IRequestHandler<PlaceOrderCommand, Result<PlaceOrderCardResult?>>
{
    public async Task<Result<PlaceOrderCardResult?>> Handle(
        PlaceOrderCommand request,
        CancellationToken cancellationToken)
    {
        // 1. Fetch user's active cart
        var cartTask = request.Request.CartId.HasValue && request.Request.CartId.Value != Guid.Empty
            ? cartService.GetCartByIdAsync(request.Request.CartId.Value, request.BearerToken, cancellationToken)
            : cartService.GetUserCartAsync(request.BearerToken, cancellationToken);

        var cart = await cartTask ?? await cartService.GetUserCartAsync(request.BearerToken, cancellationToken);
        if (cart is null || cart.Items.Count == 0)
        {
            return Result.Failure<PlaceOrderCardResult?>(
                Error.Validation("Your cart is empty. Please add items before placing an order."));
        }

        // 2. Fetch user's delivery address
        var addresses = await addressService.GetUserAddressesAsync(request.BearerToken, cancellationToken);
        var selectedAddress = (request.Request.AddressId.HasValue
            ? addresses.FirstOrDefault(a => a.Id == request.Request.AddressId.Value)
            : null)
            ?? addresses.FirstOrDefault(a => a.IsDefault)
            ?? addresses.FirstOrDefault();

        if (selectedAddress is null)
        {
            return Result.Failure<PlaceOrderCardResult?>(
                Error.NotFound("Delivery address not found or does not belong to you."));
        }

        // 3. Resolve nearest covering store & delivery fee
        var coverage = await addressService.GetNearestCoveringStoreAsync(
            selectedAddress.Latitude,
            selectedAddress.Longitude,
            cancellationToken);

        if (coverage is null || !coverage.IsServiceable)
        {
            return Result.Failure<PlaceOrderCardResult?>(
                Error.Validation("The selected address is outside our store delivery coverage area."));
        }

        // 4. Validate gift recipient details if isGift is true
        var isGift = request.Request.IsGift;
        var giftName = request.Request.GiftRecipient?.RecipientName ?? request.Request.GiftRecipientName;
        var giftPhone = request.Request.GiftRecipient?.RecipientPhone ?? request.Request.GiftRecipientPhone;

        if (isGift && (string.IsNullOrWhiteSpace(giftName) || string.IsNullOrWhiteSpace(giftPhone)))
        {
            return Result.Failure<PlaceOrderCardResult?>(
                Error.Validation("Gift recipient name and phone are required for gift orders."));
        }

        var subtotal = cart.Subtotal > 0
            ? cart.Subtotal
            : cart.Items.Sum(i => i.UnitPrice * i.Quantity);
        var deliveryFee = coverage.DeliveryFee;
        var total = subtotal + deliveryFee;
        var estimatedDeliveryAt = DateTime.UtcNow.AddMinutes(coverage.EstimatedDeliveryMinutes);

        // 5. Create Order & OrderItems
        var order = new Order
        {
            Id = Guid.CreateVersion7(),
            CustomerId = request.CustomerId,
            CartId = cart.Id != Guid.Empty ? cart.Id : (request.Request.CartId ?? Guid.NewGuid()),
            AddressId = selectedAddress.Id,
            StoreId = coverage.StoreId,
            Status = request.Request.PaymentMethod == PaymentMethod.Card
                ? OrderStatus.PendingPayment
                : OrderStatus.Placed,
            PaymentMethod = request.Request.PaymentMethod,
            PaymentProvider = request.Request.PaymentMethod == PaymentMethod.Card ? PaymentProvider.Paymob : null,
            Subtotal = subtotal,
            DeliveryFee = deliveryFee,
            Total = total,
            IsGift = isGift,
            GiftRecipientName = giftName,
            GiftRecipientPhone = giftPhone,
            EstimatedDeliveryAt = estimatedDeliveryAt,
            RecipientName = selectedAddress.RecipientName,
            RecipientPhone = selectedAddress.RecipientPhone,
            AddressLine = selectedAddress.AddressLine,
            City = selectedAddress.City,
            Area = selectedAddress.Area,
            DeliveryLatitude = selectedAddress.Latitude,
            DeliveryLongitude = selectedAddress.Longitude,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        foreach (var item in cart.Items)
        {
            order.Items.Add(new OrderItem
            {
                Id = Guid.CreateVersion7(),
                OrderId = order.Id,
                ProductId = item.ProductId,
                ProductName = item.ProductName,
                UnitPrice = item.UnitPrice,
                Quantity = item.Quantity
            });
        }

        await dbContext.Orders.AddAsync(order, cancellationToken);
        await unitOfWork.SaveChangesAsync(cancellationToken);
        logger.LogInformation("Order {OrderId} created for customer {CustomerId} with status {Status}", order.Id, request.CustomerId, order.Status);

        // 6. Handle COD vs Card as per OpenAPI 3.0.3 spec
        if (request.Request.PaymentMethod == PaymentMethod.COD)
        {
            // Cash on Delivery:
            // Order is immediately placed/final, cart is cleared, and data: null is returned
            if (order.CartId.HasValue)
            {
                await cartService.ClearCartAsync(order.CartId.Value, request.BearerToken, cancellationToken);
            }
            return Result.Success<PlaceOrderCardResult?>(null);
        }

        // Card / Paymob:
        // Order is PendingPayment. Open hosted checkout session and return session details
        var names = (request.CustomerName ?? "Valued Customer").Split(' ', StringSplitOptions.RemoveEmptyEntries);
        var firstName = names.Length > 0 ? names[0] : "Customer";
        var lastName = names.Length > 1 ? string.Join(" ", names.Skip(1)) : "User";

        var sessionRequest = new Order___Fulfillment_Service.Services.CreatePaymentSessionRequest(
            OrderId: order.Id,
            Amount: total,
            Currency: "EGP",
            PaymentProvider: PaymentProvider.Paymob,
            EstimatedDeliveryAt: estimatedDeliveryAt,
            BillingData: new Order___Fulfillment_Service.Services.BillingData(
                FirstName: firstName,
                LastName: lastName,
                Email: string.IsNullOrWhiteSpace(request.CustomerEmail) ? "customer@example.com" : request.CustomerEmail,
                PhoneNumber: string.IsNullOrWhiteSpace(request.CustomerPhone) ? selectedAddress.RecipientPhone : request.CustomerPhone,
                Country: "EGY",
                City: string.IsNullOrWhiteSpace(selectedAddress.City) ? "Cairo" : selectedAddress.City,
                Street: string.IsNullOrWhiteSpace(selectedAddress.AddressLine) ? "Street" : selectedAddress.AddressLine,
                Building: "1",
                Floor: "1",
                Apartment: "1"
            )
        );

        var sessionResult = await paymentService.CreateCardSessionAsync(sessionRequest, request.BearerToken, cancellationToken);
        var sessionId = sessionResult?.SessionId ?? $"sess_{Guid.NewGuid():N}";
        var sessionUrl = sessionResult?.SessionUrl ?? $"https://accept.paymob.com/unifiedcheckout/?publicKey=mock_pub&clientSecret=mock_sec_{order.Id:N}";
        var successUrl = $"flowery://payment/success?orderId={order.Id}";
        var cancelUrl = $"flowery://payment/cancel?orderId={order.Id}";
        var expiresAt = DateTime.UtcNow.AddMinutes(30);
        var gateway = !string.IsNullOrWhiteSpace(request.Request.PaymentGateway) ? request.Request.PaymentGateway : "Paymob";

        var cardResult = new PlaceOrderCardResult(
            OrderId: order.Id,
            Status: "PendingPayment",
            Gateway: gateway,
            SessionId: sessionId,
            SessionUrl: sessionUrl,
            SuccessUrl: successUrl,
            CancelUrl: cancelUrl,
            ExpiresAt: expiresAt,
            Amount: total,
            Currency: "EGP",
            EstimatedDeliveryAt: estimatedDeliveryAt
        );

        return Result.Success<PlaceOrderCardResult?>(cardResult);
    }
}
