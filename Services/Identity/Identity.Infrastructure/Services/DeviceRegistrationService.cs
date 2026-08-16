using Blocks.Contracts.Interfaces;
using Identity.Application.Interfaces;
using Identity.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Text;


namespace Identity.Infrastructure.Services
{
    public class DeviceRegistrationService(
    IGenericRepository<UserDevice> userDeviceRepository,
    IUnitOfWork unitOfWork)
    : IDeviceRegistrationService
    {
        public async Task RegisterAsync(
        Guid userId,
        string deviceId,
        string fcmToken,
        CancellationToken cancellationToken = default)
        {
            var currentTime = DateTime.UtcNow;

            var matchedDevices = await userDeviceRepository.FindAsync(
                device => (device.UserId == userId && device.DeviceId == deviceId) || device.FcmToken == fcmToken,
                cancellationToken);

            var currentUserDevice = matchedDevices.FirstOrDefault(
                device => device.UserId == userId && device.DeviceId == deviceId);

            var oldUsersDevices = matchedDevices.Where(
                device => device.FcmToken == fcmToken && device.Id != currentUserDevice?.Id).ToList();

            if (oldUsersDevices.Count > 0)
            {
                foreach (var oldDevice in oldUsersDevices)
                {
                    userDeviceRepository.Delete(oldDevice);
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
                    UpdatedAt = currentTime
                });
            }
            else
            {
                currentUserDevice.FcmToken = fcmToken;
                currentUserDevice.UpdatedAt = currentTime;
            }
        }
    }
}
