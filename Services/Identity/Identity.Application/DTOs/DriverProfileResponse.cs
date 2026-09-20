namespace Identity.Application.DTOs;

public sealed record DriverProfileResponse(
    Guid Id,
    string FirstName,
    string LastName,
    string? Phone,
    string? PhotoUrl
);
