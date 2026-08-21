using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Text;
using Identity.Application.Interfaces;
using Microsoft.Extensions.Configuration;

namespace Identity.Infrastructure.Services
{
    public class OtpService(IHmacService _hmacService) : IOtpService
    {
        public string GenerateOtp()
        {
            var otp = RandomNumberGenerator.GetInt32(
                0,
                1_000_000);

            return otp.ToString("D6");
        }

        public string Hash(string otp)
        {
            return _hmacService.Hash(otp);
        }

        public bool Verify(
            string otp,
            string hash)
        {
            return _hmacService.Verify(
                otp,
                hash);
        }
    }
}
