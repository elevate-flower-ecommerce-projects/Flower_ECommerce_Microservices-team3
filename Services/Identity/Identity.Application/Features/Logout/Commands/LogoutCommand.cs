using Blocks.Contracts.Common;
using MediatR;
using System;
using System.Collections.Generic;
using System.Text;

namespace Identity.Application.Features.Logout.Commands
{
    public  record LogoutCommand(Guid UserId, string DeviceId) : IRequest<Result>;
}
