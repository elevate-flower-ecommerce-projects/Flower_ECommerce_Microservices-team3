using System;
using System.Collections.Generic;
using System.Text;

namespace Identity.Application.Features.Verify_OTP
{
    public sealed record VerifyOtpResponse(
        string ResetToken,
        DateTime ExpirationDate);
}
