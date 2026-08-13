using Blocks.Contracts.Common;
using Blocks.Contracts.Interfaces;
using Blocks.Domain.Errors;
using Identity.Application.DTOs;
using Identity.Application.Features.Login.DTOs;
using Identity.Application.Features.RefreshTokens.Commands;
using Identity.Application.Interfaces;
using Identity.Application.Settings;
using Identity.Domain.Entities;
using Identity.Domain.Enums;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;

namespace Identity.Application.Features.Login.Commands;

using DriverApplication = Identity.Domain.Entities.DriverApplication;

public class LoginCommandHandler(
    IGenericRepository<User> userRepository,
    IGenericRepository<LoginAttempt> loginAttemptRepository,
    IGenericRepository<DriverApplication> driverAppRepository,
    IPasswordService passwordService,
    ITokenService tokenService,
    IDeviceRegistrationService deviceRegistrationService,
    IOptions<JwtSettings> jwtSettings,
    IMediator mediator,
    IUnitOfWork unitOfWork)
    : IRequestHandler<LoginCommand, Result<LoginResponseDto>>
{
    private const string GenericAuthError = "Invalid email or password.";
    private const int MaxFailedAttempts = 5;
    private const int LockoutWindowMinutes = 15;

    public async Task<Result<LoginResponseDto>> Handle(
        LoginCommand request,
        CancellationToken cancellationToken)
    {
        var normalizedEmail = request.Email.Trim().ToLowerInvariant();

        var now = DateTime.UtcNow;
        var windowStart = now.AddMinutes(-LockoutWindowMinutes);

        var emailAttempts = await loginAttemptRepository.GetQueryable()
            .AsNoTracking()
            .Where(a => a.Email == normalizedEmail && a.AttemptedAt >= windowStart)
            .Select(a => new { a.IsSuccessful, a.AttemptedAt })
            .ToListAsync(cancellationToken);

        var successTimes = emailAttempts
            .Where(a => a.IsSuccessful)
            .Select(a => a.AttemptedAt)
            .ToList();

        DateTime? lastSuccessAt = successTimes.Count > 0 ? successTimes.Max() : null;

        var failedByEmailCount = emailAttempts.Count(a =>
            !a.IsSuccessful && (lastSuccessAt is null || a.AttemptedAt > lastSuccessAt));

        if (failedByEmailCount >= MaxFailedAttempts)
        {
            return Result.Failure<LoginResponseDto>(
                Error.TooManyRequests("Too many failed login attempts. Please try again later."));
        }

        
        var failedByIpCount = await loginAttemptRepository.GetQueryable()
            .CountAsync(
                a => a.IpAddress == request.IpAddress
                  && !a.IsSuccessful
                  && a.AttemptedAt >= windowStart,
                cancellationToken);

        if (failedByIpCount >= MaxFailedAttempts * 3)
        {
            return Result.Failure<LoginResponseDto>(
                Error.TooManyRequests("Too many failed login attempts. Please try again later."));
        }

      
        var user = await userRepository.GetQueryable()
            .AsNoTracking()
            .Where(u => u.Email == normalizedEmail)
            .Select(u => new
            {
                u.Id,
                u.Email,
                u.FirstName,
                u.LastName,
                u.HashPassword,
                u.Role,
                u.IsActive
            })
            .FirstOrDefaultAsync(cancellationToken);

        // ── 3. Verify Password ──
        if (user is null || !passwordService.Verify(request.Password, user.HashPassword))
        {
            loginAttemptRepository.Add(new LoginAttempt
            {
                Email = normalizedEmail,
                IpAddress = request.IpAddress,
                IsSuccessful = false,
                AttemptedAt = DateTime.UtcNow
            });

            
            await unitOfWork.SaveChangesAsync(CancellationToken.None);

            return Result.Failure<LoginResponseDto>(Error.Unauthorized(GenericAuthError));
        }

        // ── 4. Check Active Status ──
        if (!user.IsActive)
        {
            return Result.Failure<LoginResponseDto>(Error.Unauthorized(GenericAuthError));
        }

        // ── 5. Check Role (Customer or Driver) ──
        if (user.Role != UserRole.Customer && user.Role != UserRole.Driver)
        {
            return Result.Failure<LoginResponseDto>(Error.Unauthorized(GenericAuthError));
        }

        // ── 6. Driver Status Lookup ──
        string? driverStatus = null;
        if (user.Role == UserRole.Driver)
        {
           
            var latestStatus = await driverAppRepository.GetQueryable()
                .AsNoTracking()
                .Where(d => d.UserId == user.Id)
                .OrderByDescending(d => d.CreatedAt)
                .ThenByDescending(d => d.Id)
                .Select(d => (DriverApplicationStatus?)d.Status)
                .FirstOrDefaultAsync(cancellationToken);

            driverStatus = latestStatus?.ToString();
        }

        // ── 7. Generate Tokens & Save Refresh Token ──
        await unitOfWork.BeginTransactionAsync(cancellationToken);
        try
        {
            var userTokenDto = new UserTokenDto(user.Id, user.Email, user.Role, user.IsActive);
            var accessToken = tokenService.GenerateAccessToken(userTokenDto);
            var refreshTokenValue = tokenService.GenerateRefreshToken();

            await mediator.Send(new SaveRefreshTokenCommand(
                refreshTokenValue,
                user.Id,
                DateTime.UtcNow.AddDays(jwtSettings.Value.RefreshTokenExpirationDays),
                request.DeviceId),
                cancellationToken);

            loginAttemptRepository.Add(new LoginAttempt
            {
                Email = normalizedEmail,
                IpAddress = request.IpAddress,
                IsSuccessful = true,
                AttemptedAt = DateTime.UtcNow
            });

            // ── 8. Save / Update FCM Token ──
            if (!string.IsNullOrWhiteSpace(request.DeviceId) && !string.IsNullOrWhiteSpace(request.FcmToken))
            {
                await deviceRegistrationService.RegisterAsync(
                    user.Id,
                    request.DeviceId,
                    request.FcmToken,
                    cancellationToken);
            }

            await unitOfWork.CommitTransactionAsync(cancellationToken);

            var expiresIn = jwtSettings.Value.AccessTokenExpirationMinutes * 60;
            var fullName = $"{user.FirstName} {user.LastName}".Trim();

            var loginUserDto = new LoginUserDto(
                user.Id,
                user.Email,
                fullName,
                user.Role.ToString(),
                user.IsActive,
                driverStatus);

            return Result.Success(new LoginResponseDto(
                accessToken,
                refreshTokenValue,
                expiresIn,
                driverStatus,
                loginUserDto));
        }
        catch
        {
            await unitOfWork.RollbackTransactionAsync();
            throw;
        }
    }
}