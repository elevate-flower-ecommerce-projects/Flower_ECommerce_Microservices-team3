using Blocks.Contracts.Common;
using Blocks.Contracts.Interfaces;
using Blocks.Domain.Errors;
using Identity.Application.Features.Profile.DTOs;
using Identity.Domain.Entities;
using MediatR;
using System;
using System.Collections.Generic;
using System.Text;

using Identity.Application.Interfaces;

namespace Identity.Application.Features.Profile.GetProfile.Queries.QueryHandlers
{
    public class GetProfileQueryHandler(
        IGenericRepository<User> userRepository,
        IFileService fileService)
    : IRequestHandler<GetProfileQuery, Result<ProfileResponseDTO>>
    {
        public async Task<Result<ProfileResponseDTO>> Handle(
            GetProfileQuery request,
            CancellationToken cancellationToken)
        {
            var user = await userRepository.GetByIdAsync(request.UserId, cancellationToken);

            if (user is null || user.DeletedAt != null)
            {
                return Result.Failure<ProfileResponseDTO>(Error.NotFound("User not found."));
            }

            var resolvedPhotoUrl = fileService.GetPublicUrl(user.PhotoUrl);

            var response = new ProfileResponseDTO(
                user.Id,
                $"{user.FirstName} {user.LastName}".Trim(),
                user.Email,
                user.Phone,
                user.Gender.ToString(),
                resolvedPhotoUrl
            );

            return Result.Success(response);
        }
    }
}
