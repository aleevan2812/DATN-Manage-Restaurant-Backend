using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Application.Common.Interfaces;
using Core.Const;
using Core.Dtos;
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

    public string GenerateAccessToken(int userId, string role, string jwtTokenId)
    {
        var claims = new[]
        {
            new Claim("userId", userId.ToString()),
            new Claim("role", role),
            new Claim("tokenType", "AccessToken"),
            new Claim("jwtTokenId", jwtTokenId),
            new Claim(JwtRegisteredClaimNames.Exp,
                new DateTimeOffset(DateTime.UtcNow.AddMinutes(15)).ToUnixTimeSeconds().ToString()),
            new Claim(JwtRegisteredClaimNames.Iat,
                new DateTimeOffset(DateTime.UtcNow).ToUnixTimeSeconds().ToString())
        };

        var key = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(_accessTokenSecret));
        var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var jwtSecurityToken = new JwtSecurityToken(
            null, // Thay thế bằng nhà phát hành của bạn
            null, // Thay thế bằng đối tượng của bạn
            claims,
            expires: DateTime.UtcNow.AddMinutes(15),
            signingCredentials: credentials
        );

        var token = new JwtSecurityTokenHandler().WriteToken(jwtSecurityToken);
        return token;
    }

    public ClaimsPrincipal ValidateTokenAndGetClaims(string token)
    {
        var tokenHandler = new JwtSecurityTokenHandler();
        var keyBytes = Encoding.ASCII.GetBytes(_refreshTokenSecret);
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

    public async Task<string> GenerateRefreshToken(int userId, string jwtTokenId, string role,
        DateTime? expiration = null)
    {
        var claims = new[]
        {
            new Claim("userId", userId.ToString()),
            new Claim("tokenType", "RefreshToken"),
            new Claim("role", role),
            new Claim("jwtTokenId", jwtTokenId),
            new Claim(JwtRegisteredClaimNames.Exp,
                new DateTimeOffset(expiration ?? DateTime.UtcNow.AddHours(24)).ToUnixTimeSeconds().ToString()),
            new Claim(JwtRegisteredClaimNames.Iat,
                new DateTimeOffset(DateTime.UtcNow).ToUnixTimeSeconds().ToString())
        };

        var key = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(_refreshTokenSecret));
        var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var jwtSecurityToken = new JwtSecurityToken(
            null, // Thay thế bằng nhà phát hành của bạn
            null, // Thay thế bằng đối tượng của bạn
            claims,
            expires: expiration ?? DateTime.UtcNow.AddHours(24),
            signingCredentials: credentials
        );

        var token = new JwtSecurityTokenHandler().WriteToken(jwtSecurityToken);

        if (string.Equals(role, Role.Guest)) return token;
        
        _context.RefreshTokens.Add(new RefreshToken
        {
            Token = token,
            IsValid = true,
            AccountId = userId,
            ExpiresAt = DateTime.UtcNow.AddHours(24),
            JwtTokenId = jwtTokenId
        });

        await _context.SaveChangesAsync(default);

        return token;
    }

    public async Task<TokenDto> RefreshToken(string oldRefreshToken)
    {
        /*Find an existing refresh token*/
        var existingRefreshToken =
            await _context.RefreshTokens.FirstOrDefaultAsync(u => u.Token == oldRefreshToken);

        if (existingRefreshToken == null) return new TokenDto();

        /*Compare data from existing refresh and access token provided and if there is any missmatch then consider it as a fraud*/
        var isTokenValid = IsValidToken(oldRefreshToken, existingRefreshToken.AccountId,
            existingRefreshToken.JwtTokenId);
        if (!isTokenValid)
        {
            await MarkTokenAsInvalid(existingRefreshToken);
            return new TokenDto();
        }

        /*When someone tries to use not valid refresh token, fraud possible*/
        if (!existingRefreshToken.IsValid)
        {
            await MarkAllTokenInChainAsInvalid(existingRefreshToken.AccountId);
            return new TokenDto();
        }

        /*If just expired then mark as invalid and return empty*/
        if (existingRefreshToken.ExpiresAt < DateTime.UtcNow)
        {
            await MarkTokenAsInvalid(existingRefreshToken);
            return new TokenDto();
        }

        var applicationUser = _context.Accounts.FirstOrDefault(u => u.Id == existingRefreshToken.AccountId);
        if (applicationUser == null)
            return new TokenDto();

        /*replace old refresh with a new one with updated expire date*/
        // await MarkAllTokenInChainAsInvalid(existingRefreshToken.AccountId);
        var newRefreshToken =
            await GenerateRefreshToken(existingRefreshToken.AccountId, existingRefreshToken.JwtTokenId,
                applicationUser.Role);

        /*revoke existing refresh token*/
        await MarkTokenAsInvalid(existingRefreshToken);

        /*generate new access token*/
        var newAccessToken =
            GenerateAccessToken(applicationUser.Id, applicationUser.Role, existingRefreshToken.JwtTokenId);

        return new TokenDto
        {
            AccessToken = newAccessToken,
            RefreshToken = newRefreshToken
        };
    }

    private bool IsValidToken(string token, int expectedUserId, string expectedTokenId)
    {
        try
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var jwt = tokenHandler.ReadJwtToken(token);
            var jwtTokenId = jwt.Claims.FirstOrDefault(u => u.Type == "jwtTokenId")?.Value;
            var userId = jwt.Claims.FirstOrDefault(u => u.Type == "userId")?.Value;
            return userId == expectedUserId.ToString() && jwtTokenId == expectedTokenId;
        }
        catch
        {
            return false;
        }
    }


    private Task MarkAllTokenInChainAsInvalid(int userId)
    {
        _context.RefreshTokens.Where(u => u.AccountId == userId)
            .ExecuteUpdate(u => u.SetProperty(refreshToken => refreshToken.IsValid, false));
        return _context.SaveChangesAsync(default);
    }

    private Task MarkTokenAsInvalid(RefreshToken refreshToken)
    {
        refreshToken.IsValid = false;
        return _context.SaveChangesAsync(default);
    }
}