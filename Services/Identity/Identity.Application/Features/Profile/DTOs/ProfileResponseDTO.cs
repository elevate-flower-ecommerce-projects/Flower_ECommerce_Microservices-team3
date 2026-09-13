using System;
using System.Collections.Generic;
using System.Text;

namespace Identity.Application.Features.Profile.DTOs
{
    public record ProfileResponseDTO(
    Guid Id,
    string FullName,
    string Email,
    string Phone,
    string Gender,
    string? PhotoUrl
    );
}
