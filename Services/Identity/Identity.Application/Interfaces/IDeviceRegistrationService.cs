namespace Identity.Application.Interfaces
{
    public interface IDeviceRegistrationService
    {
       
        Task<bool> RegisterAsync(
            Guid userId,
            string deviceId,
            string fcmToken,
            DateTime refreshTokenExpiresAt,
            CancellationToken cancellationToken = default);

       
        Task UnregisterAsync(
            Guid userId,
            string deviceId,
            CancellationToken cancellationToken = default);

        Task<bool> UpdateFcmTokenAsync(
            Guid userId,
            string deviceId,
            string fcmToken,
            CancellationToken cancellationToken = default);

        Task<bool> SetNotificationsEnabledAsync(
            Guid userId,
            string deviceId,
            bool enabled,
            CancellationToken cancellationToken = default);
    }
}
