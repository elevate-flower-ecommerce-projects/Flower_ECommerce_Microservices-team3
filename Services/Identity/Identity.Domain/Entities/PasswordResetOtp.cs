using System;
using Blocks.Domain.Entities;

namespace Identity.Domain.Entities;

public class PasswordResetOtp : AuditEntity
{
    public string OtpHash { get; private set; } = null!;
    public DateTime ExpiresAt { get; private set; }
    public DateTime LastSentAt { get; private set; }
    public int AttemptsRemaining { get; private set; }
    public bool IsUsed { get; private set; }

    #region relationship (1 - M)
    public Guid UserId { get; private set; }
    public User User { get; private set; } = null!;
    #endregion

    private PasswordResetOtp()
    {
    }

    public PasswordResetOtp(
        Guid userId,
        string otpHash,
        DateTime createdAt)
    {
        UserId = userId;
        OtpHash = otpHash;

        CreatedAt = createdAt;
        LastSentAt = createdAt;

        ExpiresAt = createdAt.AddMinutes(10);

        AttemptsRemaining = 5;
        IsUsed = false;
    }

    public bool IsExpired(DateTime now)
    {
        return now >= ExpiresAt;
    }

    public bool CanResend(DateTime now)
    {
        return now >= LastSentAt.AddSeconds(30);
    }

    public void DecreaseAttempt()
    {
        if (AttemptsRemaining > 0)
            AttemptsRemaining--;

        if (AttemptsRemaining == 0)
            IsUsed = true;
    }

    public void MarkAsUsed()
    {
        IsUsed = true;
    }
}