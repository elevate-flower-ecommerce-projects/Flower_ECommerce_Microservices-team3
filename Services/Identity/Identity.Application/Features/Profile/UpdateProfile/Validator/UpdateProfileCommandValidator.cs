using Blocks.Contracts.Interfaces;
using FluentValidation;
using Identity.Application.Features.Profile.UpdateProfile.Commands;
using Identity.Domain.Entities;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Identity.Application.Features.Profile.UpdateProfile.Validator;

public class UpdateProfileCommandValidator : AbstractValidator<UpdateProfileCommand>
{
    private readonly IGenericRepository<User> _userRepository;

    public UpdateProfileCommandValidator(IGenericRepository<User> userRepository)
    {
        _userRepository = userRepository;

        RuleFor(x => x.FullName)
            .MinimumLength(3).WithMessage("Full name must be at least 3 characters.")
            .When(x => !string.IsNullOrWhiteSpace(x.FullName));

        RuleFor(x => x.Email)
            .EmailAddress().WithMessage("Invalid email format.")
            .MustAsync(BeUniqueEmail).WithMessage("Email is already in use by another account.")
            .When(x => !string.IsNullOrWhiteSpace(x.Email));

        RuleFor(x => x.Phone)
            .Matches(@"^01[0125]\d{8}$").WithMessage("Invalid Egyptian mobile number format. It should be 11 digits starting with 010, 011, 012, or 015.")
            .MustAsync(BeUniquePhone).WithMessage("Phone number is already in use by another account.")
            .When(x => !string.IsNullOrWhiteSpace(x.Phone));

        RuleFor(x => x.Photo)
            .Must(BeAValidImage!).WithMessage("Only image files (JPEG, PNG, JPG, WEBP, GIF, BMP) are allowed.")
            .Must(file => file!.Length <= 10 * 1024 * 1024).WithMessage("Photo must not exceed 10 MB.")
            .When(x => x.Photo != null);
    }

    private async Task<bool> BeUniqueEmail(UpdateProfileCommand command, string email, CancellationToken cancellationToken)
    {
        var normalizedEmail = email.Trim().ToLowerInvariant();
        var exists = await _userRepository.GetQueryable()
            .AnyAsync(u => u.Email.ToLower() == normalizedEmail && u.Id != command.UserId, cancellationToken);

        return !exists;
    }

    private async Task<bool> BeUniquePhone(UpdateProfileCommand command, string phone, CancellationToken cancellationToken)
    {
        var exists = await _userRepository.GetQueryable()
            .AnyAsync(u => u.Phone == phone && u.Id != command.UserId, cancellationToken);

        return !exists;
    }

    private bool BeAValidImage(IFormFile file)
    {
        if (file == null) return false;
        var allowedExtensions = new[] { ".jpg", ".jpeg", ".png", ".webp", ".bmp", ".gif" };
        var extension = Path.GetExtension(file.FileName).ToLowerInvariant();
        if (allowedExtensions.Contains(extension)) return true;
        if (!string.IsNullOrEmpty(file.ContentType) && file.ContentType.StartsWith("image/", StringComparison.OrdinalIgnoreCase)) return true;
        return false;
    }
}