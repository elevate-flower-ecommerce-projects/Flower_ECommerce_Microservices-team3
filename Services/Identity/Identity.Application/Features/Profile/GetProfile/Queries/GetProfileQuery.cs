using Blocks.Contracts.Common;
using Identity.Application.Features.Profile.DTOs;
using MediatR;
using System;
using System.Collections.Generic;
using System.Text;

namespace Identity.Application.Features.Profile.GetProfile.Queries
{
    public sealed record GetProfileQuery(Guid UserId) : IRequest<Result<ProfileResponseDTO>>;
}
