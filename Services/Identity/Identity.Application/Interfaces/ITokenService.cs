using Identity.Application.DTOs;

namespace Identity.Application.Interfaces;

public interface ITokenService
{
    string GenerateAccessToken(UserTokenDto user);
    string GenerateRefreshToken();
}
