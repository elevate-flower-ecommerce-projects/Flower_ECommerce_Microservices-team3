namespace Order___Fulfillment_Service.Entities.Enums;

public enum OrderStatus
{
    Placed,
    PendingPayment,
    PaymentFailed,
    Preparing,
    OutForDelivery,
    Delivered,
    Cancelled
}
