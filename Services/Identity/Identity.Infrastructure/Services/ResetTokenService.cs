using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Text;
using Identity.Application.Interfaces;
using Microsoft.Extensions.Configuration;

namespace Identity.Infrastructure.Services
{
    public sealed class ResetTokenService(IHmacService _hmacService) 
        : IResetTokenService
    {
        public string Generate()
        {
            var tokenBytes =
                RandomNumberGenerator.GetBytes(32);

            return Convert.ToBase64String(tokenBytes)
                .Replace("+", "-")
                .Replace("/", "_")
                .Replace("=", "");
        }

        public string Hash(string token)
        {
            return _hmacService.Hash(token);
        }

        public bool Verify(
            string token,
            string tokenHash)
        {
            return _hmacService.Verify(
                token,
                tokenHash);
        }
    }
}
