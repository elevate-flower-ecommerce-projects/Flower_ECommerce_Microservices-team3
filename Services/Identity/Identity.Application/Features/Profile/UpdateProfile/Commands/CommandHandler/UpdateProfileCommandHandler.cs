using Blocks.Contracts.Common;
using Blocks.Contracts.Interfaces;
using Blocks.Domain.Errors;
using Identity.Application.Features.Profile.DTOs;
using Identity.Application.Features.Profile.UpdateProfile.Commands;
using Identity.Application.Interfaces;
using Identity.Domain.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System.Threading;
using System.Threading.Tasks;

namespace Identity.Application.Features.Profile.UpdateProfile.Commands.CommandHandler
{
    public class UpdateProfileCommandHandler(
    IGenericRepository<User> userRepository,
    IFileService fileService,
    IUnitOfWork unitOfWork)
    : IRequestHandler<UpdateProfileCommand, Result<ProfileResponseDTO>>
    {
        public async Task<Result<ProfileResponseDTO>> Handle(
            UpdateProfileCommand request,
            CancellationToken cancellationToken)
        {
            var user = await userRepository.GetQueryable()
                .FirstOrDefaultAsync(u => u.Id == request.UserId, cancellationToken);

            if (user is null || user.DeletedAt != null)
            {
                return Result.Failure<ProfileResponseDTO>(Error.NotFound("User not found."));
            }

            if (!string.IsNullOrWhiteSpace(request.FullName))
            {
                var names = request.FullName.Trim().Split(' ', 2);
                user.FirstName = names[0];
                user.LastName = names.Length > 1 ? names[1] : string.Empty;
            }

            if (!string.IsNullOrWhiteSpace(request.Email))
            {
                var newEmail = request.Email.Trim().ToLowerInvariant();
                if (!string.Equals(user.Email, newEmail, StringComparison.OrdinalIgnoreCase))
                {
                    var emailExists = await userRepository.GetQueryable()
                        .AnyAsync(u => u.Email == newEmail && u.Id != user.Id && u.DeletedAt == null, cancellationToken);
                    if (emailExists)
                    {
                        return Result.Failure<ProfileResponseDTO>(Error.Conflict("Email is already in use by another account."));
                    }
                    user.Email = newEmail;
                }
            }

            if (!string.IsNullOrWhiteSpace(request.Phone))
                user.Phone = request.Phone.Trim();

            if (request.Gender.HasValue)
                user.Gender = request.Gender.Value;

            if (request.Photo != null)
            {
                if (!string.IsNullOrWhiteSpace(user.PhotoUrl))
                {
                    await fileService.DeleteFileAsync(user.PhotoUrl, cancellationToken);
                }

                var photoUrl = await fileService.UploadFileAsync(request.Photo, "ProfilePictures", cancellationToken);
                user.PhotoUrl = photoUrl;
            }
            else if (!string.IsNullOrWhiteSpace(request.PhotoUrl))
            {
                if (request.PhotoUrl.StartsWith("data:image", StringComparison.OrdinalIgnoreCase) ||
                    (request.PhotoUrl.Length > 200 && !request.PhotoUrl.StartsWith("http", StringComparison.OrdinalIgnoreCase)))
                {
                    var uploadedUrl = await fileService.UploadBase64Async(request.PhotoUrl, "ProfilePictures", cancellationToken);
                    user.PhotoUrl = uploadedUrl;
                }
                else
                {
                    user.PhotoUrl = request.PhotoUrl;
                }
            }

            userRepository.Update(user);
            await unitOfWork.SaveChangesAsync(cancellationToken);

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
