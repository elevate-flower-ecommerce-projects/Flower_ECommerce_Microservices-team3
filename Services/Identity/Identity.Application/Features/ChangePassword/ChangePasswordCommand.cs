using MediatR;
using Blocks.Contracts.Common;
namespace Identity.Application.Features.ChangePassword
{
    public record ChangePasswordCommand(
       Guid UserId,
    string CurrentPassword,
    string NewPassword
        ) :IRequest<Result>;
    
}
