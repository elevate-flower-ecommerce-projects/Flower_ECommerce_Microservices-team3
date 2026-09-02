using Blocks.Domain.Entities;
using Order___Fulfillment_Service.Entities.Enums;

namespace Order___Fulfillment_Service.Entities;

public class Order : AuditEntity
{
    public Guid UserId { get; set; }
    public Guid? CartId { get; set; }
    public Guid AddressId { get; set; }
    public Guid StoreId { get; set; }
    public OrderStatus Status { get; set; } = OrderStatus.Placed;
    public PaymentMethod PaymentMethod { get; set; }
    public PaymentGateway? PaymentGateway { get; set; }
    public decimal Subtotal { get; set; }
    public decimal DeliveryFee { get; set; }
    public decimal Total { get; set; }
    public bool IsGift { get; set; }
    public string? GiftRecipientName { get; set; }
    public string? GiftRecipientPhone { get; set; }
    public DateTime EstimatedDeliveryAt { get; set; }

    // Delivery Address snapshot
    public string RecipientName { get; set; } = string.Empty;
    public string RecipientPhone { get; set; } = string.Empty;
    public string AddressLine { get; set; } = string.Empty;
    public string City { get; set; } = string.Empty;
    public string Area { get; set; } = string.Empty;

    public string? CancellationReason { get; set; }

    public ICollection<OrderItem> Items { get; set; } = [];
}
