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
    : IRequestHandler<PlaceOrderCommand, Result<PlaceOrderResponse>>
{
    public async Task<Result<PlaceOrderResponse>> Handle(
        PlaceOrderCommand request,
        CancellationToken cancellationToken)
    {
        // 1. Fetch user's active cart
        var cart = await cartService.GetUserCartAsync(request.BearerToken, cancellationToken);
        if (cart is null || cart.Items.Count == 0)
        {
            return Result.Failure<PlaceOrderResponse>(
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
            return Result.Failure<PlaceOrderResponse>(
                Error.Validation("Delivery address is required. Please add or select an address before checkout."));
        }

        // 3. Resolve nearest covering store & delivery fee
        var coverage = await addressService.GetNearestCoveringStoreAsync(
            selectedAddress.Latitude,
            selectedAddress.Longitude,
            cancellationToken);

        if (coverage is null || !coverage.IsServiceable)
        {
            return Result.Failure<PlaceOrderResponse>(
                Error.Validation("The selected address is outside our store delivery coverage area."));
        }

        var subtotal = cart.Subtotal > 0
            ? cart.Subtotal
            : cart.Items.Sum(i => i.UnitPrice * i.Quantity);
        var deliveryFee = coverage.DeliveryFee;
        var total = subtotal + deliveryFee;
        var estimatedDeliveryAt = DateTime.UtcNow.AddMinutes(coverage.EstimatedDeliveryMinutes);

        // 4. Create Order & OrderItems
        var order = new Order
        {
            Id = Guid.CreateVersion7(),
            CustomerId = request.CustomerId,
            CartId = cart.Id,
            AddressId = selectedAddress.Id,
            StoreId = coverage.StoreId,
            Status = request.Request.PaymentMethod == PaymentMethod.Card
                ? OrderStatus.PendingPayment
                : OrderStatus.Preparing,
            PaymentMethod = request.Request.PaymentMethod,
            PaymentProvider = request.Request.PaymentMethod == PaymentMethod.Card ? PaymentProvider.Paymob : null,
            Subtotal = subtotal,
            DeliveryFee = deliveryFee,
            Total = total,
            IsGift = request.Request.IsGift,
            GiftRecipientName = request.Request.GiftRecipientName,
            GiftRecipientPhone = request.Request.GiftRecipientPhone,
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

        // 5. Handle payment method
        string? paymentUrl = null;
        string? sessionId = null;

        if (request.Request.PaymentMethod == PaymentMethod.Card)
        {
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
            if (sessionResult is not null)
            {
                paymentUrl = sessionResult.SessionUrl;
                sessionId = sessionResult.SessionId;
            }
        }
        else
        {
            // Cash on Delivery: record payment and immediately clear cart
            await paymentService.CreateCodPaymentAsync(order.Id, total, "EGP", request.BearerToken, cancellationToken);
            await cartService.ClearCartAsync(cart.Id, request.BearerToken, cancellationToken);
        }

        return Result.Success(new PlaceOrderResponse(
            OrderId: order.Id,
            Status: order.Status,
            PaymentMethod: order.PaymentMethod,
            Subtotal: order.Subtotal,
            DeliveryFee: order.DeliveryFee,
            Total: order.Total,
            EstimatedDeliveryAt: order.EstimatedDeliveryAt,
            PaymentUrl: paymentUrl,
            SessionId: sessionId
        ));
    }
}
