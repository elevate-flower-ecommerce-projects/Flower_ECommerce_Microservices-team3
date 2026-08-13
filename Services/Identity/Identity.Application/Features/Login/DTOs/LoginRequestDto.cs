namespace Identity.Application.Features.Login.DTOs;

public record LoginRequestDto(
    string Email,
    string Password,
    string? DeviceId,
    string? FcmToken);
