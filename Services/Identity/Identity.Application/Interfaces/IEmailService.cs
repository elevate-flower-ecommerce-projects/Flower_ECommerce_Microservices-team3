using System;
using System.Collections.Generic;
using System.Text;

namespace Identity.Application.Interfaces
{
    public interface IEmailService
    {
        Task SendPasswordChangedEmailAsync(string toEmail, string userName, CancellationToken cancellationToken = default);
    }
}
