using Blocks.Contracts.Common;
using Identity.Application.Features.Profile.DTOs;
using Identity.Domain.Enums;
using MediatR;
using Microsoft.AspNetCore.Http;
using System;

namespace Identity.Application.Features.Profile.UpdateProfile.Commands
{
    public sealed record UpdateProfileCommand(
    Guid UserId,
    string? FullName,
    string? Email,
    string? Phone,
    Gender? Gender,
    IFormFile? Photo
    ) : IRequest<Result<ProfileResponseDTO>>;
}

