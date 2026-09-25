using Blocks.Contracts.Interfaces;
using Identity.Application.Interfaces;
using Identity.Domain.Entities;

namespace Identity.Infrastructure.Services
{
    public class DeviceRegistrationService(
        IGenericRepository<UserDevice> userDeviceRepository,
        IUnitOfWork unitOfWork)
        : IDeviceRegistrationService
    {
        public async Task<bool> RegisterAsync(
            Guid userId,
            string deviceId,
            string fcmToken,
            DateTime refreshTokenExpiresAt,
            CancellationToken cancellationToken = default)
        {
            var currentTime = DateTime.UtcNow;

            var matchedDevices = await userDeviceRepository.FindAsync(
                device => (device.UserId == userId && device.DeviceId == deviceId)
                          || device.FcmToken == fcmToken,
                cancellationToken);

            var currentUserDevice = matchedDevices.FirstOrDefault(
                device => device.UserId == userId && device.DeviceId == deviceId);

            // Find any other devices that currently hold this exact FcmToken
            var staleDevices = matchedDevices.Where(
                device => device.FcmToken == fcmToken && device.Id != currentUserDevice?.Id).ToList();

            // Revoke the token from stale devices and persist before assigning the token to the new/current device
            // to prevent unique index violation (UX_UserDevices_FcmToken) in SQL Server.
            if (staleDevices.Count > 0)
            {
                foreach (var stale in staleDevices)
                {
                    stale.IsActive = false;
                    stale.FcmToken = $"revoked_{Guid.NewGuid():N}";
                    stale.UpdatedAt = currentTime;
                }

                await unitOfWork.SaveChangesAsync(cancellationToken);
            }

            if (currentUserDevice is null)
            {
                userDeviceRepository.Add(new UserDevice
                {
                    UserId = userId,
                    DeviceId = deviceId,
                    FcmToken = fcmToken,
                    IsActive = true,
                    NotificationsEnabled = true,
                    RefreshTokenExpiresAt = refreshTokenExpiresAt,
                    UpdatedAt = currentTime
                });

                return true;
            }
            else
            {
                currentUserDevice.FcmToken = fcmToken;
                currentUserDevice.IsActive = true;
                currentUserDevice.RefreshTokenExpiresAt = refreshTokenExpiresAt;
                currentUserDevice.UpdatedAt = currentTime;

                return currentUserDevice.NotificationsEnabled;
            }
        }

        public async Task UnregisterAsync(
            Guid userId,
            string deviceId,
            CancellationToken cancellationToken = default)
        {
            var devices = await userDeviceRepository.FindAsync(
                d => d.UserId == userId && d.DeviceId == deviceId,
                cancellationToken);

            foreach (var device in devices)
            {
                device.IsActive = false;
                device.FcmToken = $"logged_out_{Guid.NewGuid():N}";
                device.UpdatedAt = DateTime.UtcNow;
            }

            await unitOfWork.SaveChangesAsync(cancellationToken);
        }

        public async Task<bool> UpdateFcmTokenAsync(
            Guid userId,
            string deviceId,
            string fcmToken,
            CancellationToken cancellationToken = default)
        {
            var currentTime = DateTime.UtcNow;

            var matchedDevices = await userDeviceRepository.FindAsync(
                d => (d.UserId == userId && d.DeviceId == deviceId) || d.FcmToken == fcmToken,
                cancellationToken);

            var currentUserDevice = matchedDevices.FirstOrDefault(
                d => d.UserId == userId && d.DeviceId == deviceId);

            var staleDevices = matchedDevices.Where(
                d => d.FcmToken == fcmToken && d.Id != currentUserDevice?.Id).ToList();

            if (staleDevices.Count > 0)
            {
                foreach (var stale in staleDevices)
                {
                    stale.IsActive = false;
                    stale.FcmToken = $"revoked_{Guid.NewGuid():N}";
                    stale.UpdatedAt = currentTime;
                }

                await unitOfWork.SaveChangesAsync(cancellationToken);
            }

            if (currentUserDevice is null)
            {
                userDeviceRepository.Add(new UserDevice
                {
                    UserId = userId,
                    DeviceId = deviceId,
                    FcmToken = fcmToken,
                    IsActive = true,
                    NotificationsEnabled = true,
                    UpdatedAt = currentTime
                });
            }
            else
            {
                currentUserDevice.FcmToken = fcmToken;
                currentUserDevice.IsActive = true;
                currentUserDevice.UpdatedAt = currentTime;
            }

            await unitOfWork.SaveChangesAsync(cancellationToken);
            return true;
        }

        public async Task<bool> SetNotificationsEnabledAsync(
            Guid userId,
            string deviceId,
            bool enabled,
            CancellationToken cancellationToken = default)
        {
            var currentTime = DateTime.UtcNow;

            var devices = await userDeviceRepository.FindAsync(
                d => d.UserId == userId && d.DeviceId == deviceId,
                cancellationToken);

            var device = devices.FirstOrDefault();
            if (device is null)
            {
                userDeviceRepository.Add(new UserDevice
                {
                    UserId = userId,
                    DeviceId = deviceId,
                    FcmToken = $"unregistered_{Guid.NewGuid():N}",
                    IsActive = true,
                    NotificationsEnabled = enabled,
                    UpdatedAt = currentTime
                });
            }
            else
            {
                device.NotificationsEnabled = enabled;
                device.UpdatedAt = currentTime;
            }

            await unitOfWork.SaveChangesAsync(cancellationToken);
            return true;
        }
    }
}
