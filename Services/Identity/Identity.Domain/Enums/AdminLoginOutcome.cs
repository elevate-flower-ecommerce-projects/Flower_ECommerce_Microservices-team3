namespace Identity.Domain.Enums;

public enum AdminLoginOutcome
{
    Success,
    InvalidCredentials,
    AccountDisabled,
    NotAdminRole,
    RateLimited
}
