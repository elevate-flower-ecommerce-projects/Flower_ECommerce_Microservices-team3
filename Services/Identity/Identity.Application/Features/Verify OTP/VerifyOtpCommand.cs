using System;
using System.Collections.Generic;
using System.Text;
using Blocks.Contracts.Common;
using MediatR;

namespace Identity.Application.Features.Verify_OTP
{
    public sealed record VerifyOtpCommand(string Email,
                                          string Otp) 
        : IRequest<Result<VerifyOtpResponse>>;
}
