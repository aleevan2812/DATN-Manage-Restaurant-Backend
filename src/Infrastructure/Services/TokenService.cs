using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Application.Common.Interfaces;
using Core.Entities;
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

    public string GenerateAccessToken(int userId, string role, DateTime expiration)
    {
        var claims = new[]
        {
            new Claim("userId", userId.ToString()),
            new Claim("role", role),
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

    public ClaimsPrincipal ValidateTokenAndGetClaims(string token, string securityKey)
    {
        var tokenHandler = new JwtSecurityTokenHandler();
        var keyBytes = Encoding.ASCII.GetBytes(securityKey);
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
}