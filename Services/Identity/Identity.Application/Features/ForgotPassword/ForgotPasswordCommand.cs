using System;
using System.Collections.Generic;
using System.Text;
using Blocks.Contracts.Common;
using MediatR;

namespace Identity.Application.Features.ForgotPassword
{
    public sealed record ForgotPasswordCommand(string Email) 
        : IRequest<Result<ForgotPasswordResponse>>;
}
