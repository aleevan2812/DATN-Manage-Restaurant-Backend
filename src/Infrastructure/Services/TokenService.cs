using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Application.Common.Interfaces;
using Core.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;

namespace Infrastructure.Services;

public class TokenService : ITokenService
{
    private readonly string _accessTokenSecret;
    private readonly IApplicationDbContext _context;
    private readonly string _refreshTokenSecret;

    public TokenService(IApplicationDbContext context, IConfiguration configuration)
    {
        _context = context;
        _accessTokenSecret = configuration.GetValue<string>("Token:AccessTokenSecret");
        _refreshTokenSecret = configuration.GetValue<string>("Token:RefreshTokenSecret");
    }

    public string GenerateAccessToken(int userId, string role, string jwtTokenId, DateTime expiration)
    {
        var claims = new[]
        {
            new Claim("userId", userId.ToString()),
            new Claim("role", role),
            new Claim("jwtTokenId", jwtTokenId),
            new Claim("tokenType", "AccessToken"),
            new Claim(JwtRegisteredClaimNames.Exp,
                new DateTimeOffset(expiration).ToUnixTimeSeconds().ToString()),
            new Claim(JwtRegisteredClaimNames.Iat,
                new DateTimeOffset(expiration).ToUnixTimeSeconds().ToString())
        };

        var key = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(_accessTokenSecret));
        var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var jwtSecurityToken = new JwtSecurityToken(
            null, // Thay thế bằng nhà phát hành của bạn
            null, // Thay thế bằng đối tượng của bạn
            claims,
            expires: expiration,
            signingCredentials: credentials
        );

        var token = new JwtSecurityTokenHandler().WriteToken(jwtSecurityToken);
        return token;
    }

    public ClaimsPrincipal ValidateTokenAndGetClaims(string token)
    {
        var tokenHandler = new JwtSecurityTokenHandler();
        var keyBytes = Encoding.ASCII.GetBytes(_accessTokenSecret);
        var validationParameters = new TokenValidationParameters
        {
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(keyBytes),
            ValidateIssuer = false, // Đặt là true nếu token yêu cầu kiểm tra issuer
            ValidateAudience = false, // Đặt là true nếu token yêu cầu kiểm tra audience
            ValidateLifetime = true, // Bật xác thực thời gian hết hạn của token
            ClockSkew = TimeSpan.Zero // Loại bỏ độ trễ mặc định để xác thực chính xác thời gian hết hạn
        };

        try
        {
            var principal = tokenHandler.ValidateToken(token, validationParameters, out var validatedToken);
            return principal;
        }
        catch (SecurityTokenExpiredException)
        {
            Console.WriteLine("Token đã hết hạn.");
            throw;
        }
        catch (SecurityTokenInvalidSignatureException)
        {
            Console.WriteLine("Chữ ký của token không hợp lệ.");
            throw;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Token không hợp lệ: {ex.Message}");
            throw;
        }
    }

    public async Task<string> GenerateRefreshToken(int userId, string role, DateTime expiration)
    {
        var claims = new[]
        {
            new Claim("userId", userId.ToString()),
            new Claim("role", role),
            new Claim("tokenType", "RefreshToken"),
            new Claim(JwtRegisteredClaimNames.Exp,
                new DateTimeOffset(expiration).ToUnixTimeSeconds().ToString()),
            new Claim(JwtRegisteredClaimNames.Iat,
                new DateTimeOffset(expiration).ToUnixTimeSeconds().ToString())
        };

        var key = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(_refreshTokenSecret));
        var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var jwtSecurityToken = new JwtSecurityToken(
            null, // Thay thế bằng nhà phát hành của bạn
            null, // Thay thế bằng đối tượng của bạn
            claims,
            expires: expiration,
            signingCredentials: credentials
        );

        var token = new JwtSecurityTokenHandler().WriteToken(jwtSecurityToken);

        _context.RefreshTokens.Add(new RefreshToken
        {
            Token = token,
            IsValid = true,
            AccountId = userId,
            ExpiresAt = expiration
        });

        await _context.SaveChangesAsync(default);

        return token;
    }

    // public async Task<string> RefreshToken(string oldRefreshToken)
    // {
    //     /*Find an existing refresh token*/
    //     var existingRefreshToken =
    //         await _context.RefreshTokens.FirstOrDefaultAsync(u => u.Token == oldRefreshToken);
    //
    //     if (existingRefreshToken == null) return string.Empty;
    // }

    private bool IsValidAccessToken(string accessToken, string expectedUserId, string expectedTokenId)
    {
        try
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var jwt = tokenHandler.ReadJwtToken(accessToken);
            var jwtTokenId = jwt.Claims.FirstOrDefault(u => u.Type == "jwtTokenId")?.Value;
            var userId = jwt.Claims.FirstOrDefault(u => u.Type == "userId")?.Value;
            return userId == expectedUserId && jwtTokenId == expectedTokenId;
        }
        catch
        {
            return false;
        }
    }


    private async Task MarkAllTokenInChainAsInvalid(int userId, int tokenId)
    {
        await _context.RefreshTokens
            .Where(u => u.AccountId == userId)
            .ExecuteUpdateAsync(u => u.SetProperty(refreshToken => refreshToken.IsValid, false));
    }

    private Task MarkTokenAsInvalid(RefreshToken refreshToken)
    {
        refreshToken.IsValid = false;
        return _context.SaveChangesAsync(default);
    }
}