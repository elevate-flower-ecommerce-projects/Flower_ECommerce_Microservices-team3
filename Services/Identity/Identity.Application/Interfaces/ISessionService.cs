using System;
using System.Collections.Generic;
using System.Text;

namespace Identity.Application.Interfaces
{
    public interface ISessionService
    {
        Task RevokeAllUserSessionsAsync(Guid userId, CancellationToken cancellationToken = default);
    }
}
