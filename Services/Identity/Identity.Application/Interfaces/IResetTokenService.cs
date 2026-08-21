using System;
using System.Collections.Generic;
using System.Text;

namespace Identity.Application.Interfaces
{
    public interface IResetTokenService
    {
        string Generate();
        string Hash(string token);
        bool Verify(string token, string tokenHash);
    }
}
