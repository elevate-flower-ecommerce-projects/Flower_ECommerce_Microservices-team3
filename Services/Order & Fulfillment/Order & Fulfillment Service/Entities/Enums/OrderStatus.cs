using System.Text.Json.Serialization;

namespace Order___Fulfillment_Service.Entities.Enums;

[JsonConverter(typeof(JsonStringEnumConverter))]
public enum OrderStatus
{
    Placed,
    PendingPayment,
    PaymentFailed,
    Preparing,
    PickedUp,
    OutForDelivery,
    AwaitingDeliveryConfirmation,
    Delivered,
    Cancelled
}
