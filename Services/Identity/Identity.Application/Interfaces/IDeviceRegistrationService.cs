using System;
using System.Collections.Generic;
using System.Text;

namespace Identity.Application.Interfaces
{
    public interface IDeviceRegistrationService
    {
        Task RegisterAsync(
        Guid userId,
        string deviceId,
        string fcmToken,
        CancellationToken cancellationToken = default);
    }
}
