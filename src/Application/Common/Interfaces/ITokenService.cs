using System.Security.Claims;

namespace Application.Common.Interfaces;

public interface ITokenService
{
    string GenerateAccessToken(int userId, string role, DateTime expiration);
    ClaimsPrincipal ValidateTokenAndGetClaims(string token, string securityKey);
    Task<string> GenerateRefreshToken(int userId, string role, DateTime expiration);
}