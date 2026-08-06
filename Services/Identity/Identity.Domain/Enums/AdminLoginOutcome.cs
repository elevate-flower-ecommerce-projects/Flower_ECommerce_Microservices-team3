namespace Identity.Domain.Enums;

public enum AdminLoginOutcome
{
    Success,
    InvalidCredentials,
    NotAdminRole,
    RateLimited
}
