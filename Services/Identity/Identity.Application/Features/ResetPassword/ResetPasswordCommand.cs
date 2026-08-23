using Blocks.Contracts.Common;
using Identity.Application.Features.ResetPassword;
using MediatR;

namespace Identity.Api.Features.Forgot_Password
{
    public sealed record ResetPasswordCommand(
        string ResetToken,
        string NewPassword,
        string ConfirmPassword) 
        : IRequest<Result<ResetPasswordResponse>>;

}
