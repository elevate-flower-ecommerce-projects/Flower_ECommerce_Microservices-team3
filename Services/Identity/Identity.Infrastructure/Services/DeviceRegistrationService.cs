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

          
            var staleDevices = matchedDevices.Where(
                device => device.FcmToken == fcmToken && device.Id != currentUserDevice?.Id).ToList();

            foreach (var stale in staleDevices)
            {
                stale.IsActive = false;
                stale.UpdatedAt = currentTime;
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

            foreach (var stale in staleDevices)
            {
                stale.IsActive = false;
                stale.UpdatedAt = currentTime;
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
                    FcmToken = string.Empty,
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

