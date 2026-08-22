using System;
using System.Collections.Generic;
using System.Text;

namespace Identity.Application.Interfaces
{
    public interface IHmacService
    {
        string Hash(string value);

        bool Verify(
            string value,
            string hash);
    }
}
