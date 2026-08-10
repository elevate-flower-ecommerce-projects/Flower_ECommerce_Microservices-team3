using MediatR;
using System;
using System.Collections.Generic;
using System.Text;

namespace Identity.Application.Features.Login.Commands
{
    public  record LogLoginAttemptCommand(
    string Email,
    string IpAddress,
    bool IsSuccessful)
    : IRequest;
}
