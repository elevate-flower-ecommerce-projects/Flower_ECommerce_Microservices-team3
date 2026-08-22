using System;
using System.Collections.Generic;
using System.Text;

namespace Identity.Application.Interfaces
{
    public interface IOtpService
    {
        string GenerateOtp();
        string Hash(string otp);
        bool Verify(
            string otp,
            string otpHash);
    }
}
