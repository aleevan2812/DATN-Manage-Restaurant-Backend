using System;
using System.Security.Claims;

namespace Application.Common.Interfaces;

public interface ITokenService
{
    string GenerateAccessToken(int userId, string role, string securityKey, DateTime expiration);
    ClaimsPrincipal ValidateTokenAndGetClaims(string token, string securityKey);
    string GenerateRefreshToken(int userId, string role, string securityKey, DateTime expiration);
}