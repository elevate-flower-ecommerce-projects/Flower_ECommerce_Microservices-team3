using Blocks.Domain.Entities;
using Payment_Service.Entities.Enums;

namespace Payment_Service.Entities
{
    public class Payment : BaseEntity
    {
        public Guid OrderId { get; set; }
        public decimal Amount { get; set; }
        public string Currency { get; set; } = "EGP";
        public PaymentMethod Method { get; set; }
        public PaymentStatus Status { get; set; } = PaymentStatus.Pending;

        // Paymob
        public string? PaymobIntentionId { get; set; }
        public string? PaymobClientSecret { get; set; }
        public string? PaymobOrderId { get; set; }
        public string? PaymobTransactionId { get; set; }
        public DateTime? PaidAt { get; set; }
    }
}