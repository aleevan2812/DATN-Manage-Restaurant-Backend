using System.Security.Claims;

namespace Application.Common.Interfaces;

public interface ITokenService
{
    string GenerateAccessToken(int userId, string role, string jwtTokenId, DateTime expiration);
    ClaimsPrincipal ValidateTokenAndGetClaims(string token);
    Task<string> GenerateRefreshToken(int userId, string role, string jwtTokenId, DateTime expiration);
}