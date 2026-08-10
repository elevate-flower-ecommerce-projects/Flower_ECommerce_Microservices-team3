using FluentValidation;

namespace Identity.Api.Features.RefreshToken;

public sealed record RefreshTokenRequestVm(string Token);

public class RefreshTokenRequestVmValidator : AbstractValidator<RefreshTokenRequestVm>
{
    public RefreshTokenRequestVmValidator()
    {
        RuleFor(x => x.Token).NotEmpty().WithMessage("Refresh token is required.");
    }
}
