namespace Identity.Application.Features.RefreshTokens.ViewModels
{
    public sealed record RefreshTokenResponseVm(
        string AccessToken,
        string RefreshToken,
        DateTime ExpiresAt);
}
