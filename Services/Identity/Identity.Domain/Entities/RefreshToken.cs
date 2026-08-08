using Blocks.Domain.Entities;

namespace Identity.Domain.Entities
{
    public class RefreshToken : BaseEntity
    {
        public Guid UserId { get; set; }
        public User User { get; set; } = null!;
        public string Token { get; set; } = string.Empty;
        public string TokenHash { get; set; } = string.Empty;
        public Guid FamilyId { get; set; }
        public string? DeviceInfo { get; set; }
        public DateTime IssuedAt { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime ExpiresAt { get; set; }
        public bool IsRevoked { get; set; } = false;
        public DateTime? RevokedAt { get; set; }
        public Guid? ReplacedByTokenId { get; set; }
    }
}
