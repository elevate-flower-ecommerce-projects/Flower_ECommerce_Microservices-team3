using Blocks.Domain.Entities;

namespace Identity.Domain.Entities;

public class UserDevice : BaseEntity
{
    public Guid UserId { get; set; }

    public string DeviceId { get; set; } = string.Empty;

    public string FcmToken { get; set; } = string.Empty;

    public bool IsActive { get; set; } = true;

    public bool NotificationsEnabled { get; set; } = true;

    public DateTime? RefreshTokenExpiresAt { get; set; }


    public DateTime UpdatedAt { get; set; }

    public User User { get; set; } = null!;
}
