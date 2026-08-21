using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Text;
using Identity.Application.Interfaces;
using Microsoft.Extensions.Configuration;

namespace Identity.Infrastructure.Services
{
    public sealed class HmacService : IHmacService
    {
        private readonly byte[] _secretKey;

        public HmacService(IConfiguration configuration)
        {
            var key = configuration["Security:HmacKey"];

            if (string.IsNullOrWhiteSpace(key))
            {
                throw new InvalidOperationException(
                    "HMAC key is not configured.");
            }

            _secretKey = Encoding.UTF8.GetBytes(key);
        }

        public string Hash(string value)
        {
            using var hmac = new HMACSHA256(_secretKey);

            var hash = hmac.ComputeHash(
                Encoding.UTF8.GetBytes(value));

            return Convert.ToHexString(hash);
        }

        public bool Verify(
            string value,
            string hash)
        {
            var computedHash = Hash(value);

            return CryptographicOperations.FixedTimeEquals(
                Convert.FromHexString(computedHash),
                Convert.FromHexString(hash));
        }
    }
}
