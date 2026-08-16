using Identity.Domain.Enums;

namespace Identity.Application.DTOs;

public sealed record UserTokenDto(
    Guid Id,
    string Email,
    UserRole Role,
    bool IsActive
);
