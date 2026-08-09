using BCrypt.Net;
using Identity.Application.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;

namespace Identity.Infrastructure.Services
{
    public class PasswordService : IPasswordService
    {
        public string Hash(string password)
        {
            return BCrypt.Net.BCrypt.HashPassword(password);
        }

        public bool Verify(string password, string passwordHash)
        {
            return BCrypt.Net.BCrypt.Verify(password, passwordHash);
        }
    }
}
