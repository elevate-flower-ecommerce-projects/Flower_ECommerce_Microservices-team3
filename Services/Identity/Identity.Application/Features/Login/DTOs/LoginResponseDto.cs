namespace Identity.Application.Features.Login.DTOs;

public record LoginUserDto(
    Guid Id,
    string Email,
    string FullName,
    string Role,
    bool IsActive,
    string? DriverStatus);

public record LoginDeviceDto(
    string DeviceId,
    bool NotificationsEnabled);

public record LoginResponseDto(
    string AccessToken,
    string RefreshToken,
    int ExpiresIn,
    string? DriverStatus,
    LoginUserDto User,
    LoginDeviceDto? Device);
