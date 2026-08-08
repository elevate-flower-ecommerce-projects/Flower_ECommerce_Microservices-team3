namespace Identity.Application.Features.AdminLogin.ViewModels
{
    public record AdminLoginResponseVm(
        string AccessToken,
        string RefreshToken,
        DateTime ExpiresAt
    );
}
