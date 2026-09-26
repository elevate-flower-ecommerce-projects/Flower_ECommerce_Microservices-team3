namespace Order___Fulfillment_Service.Features.Checkout.EstimateDelivery;

public sealed record EstimateDeliveryResponse(
    decimal? DeliveryFee,
    DateTime? EstimatedDeliveryAt
);
