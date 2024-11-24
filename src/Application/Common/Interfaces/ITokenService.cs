using System.Security.Claims;
using Core.Dtos;

namespace Application.Common.Interfaces;

public interface ITokenService
{
    string GenerateAccessToken(int userId, string role, string jwtTokenId);
    ClaimsPrincipal ValidateTokenAndGetClaims(string token);
    Task<string> GenerateRefreshToken(int userId, string jwtTokenId, string role, DateTime? expiration = null);
    Task<TokenDto> RefreshToken(string oldRefreshToken);
}