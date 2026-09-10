using Blocks.Domain.Entities;

namespace Payment_Service.Entities
{
    public class PaymentWebhookEvent : BaseEntity
    {
        public string EventId { get; set; } = string.Empty;
        public string EventType { get; set; } = string.Empty;
        public DateTime ReceivedAt { get; set; } = DateTime.UtcNow;
        public DateTime? ProcessedAt { get; set; }
    }
}
