using Blocks.Contracts.Common;
using Identity.Application.Features.Login.ViewModels;
using MediatR;
using System;
using System.Collections.Generic;
using System.Text;

namespace Identity.Application.Features.Login.Commands
{
    public record LoginOrchestrator(
        string Email,
        string Password,
        string IpAddress) : IRequest<Result<LoginResponseVM>>;
    
}
