using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.IdentityModel.Tokens;

namespace Application.Services;

public class TokenService
{
    public static string GenerateAccessToken(int userId, string role, string securityKey, DateTime expiration)
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

        var key = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(securityKey));
        var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var token = new JwtSecurityToken(
            null, // Thay thế bằng nhà phát hành của bạn
            null, // Thay thế bằng đối tượng của bạn
            claims,
            expires: expiration,
            signingCredentials: credentials
        );
        return new JwtSecurityTokenHandler().WriteToken(token);
    }

    public static ClaimsPrincipal ValidateTokenAndGetClaims(string token, string securityKey)
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

    public static string GenerateRefreshToken(int userId, string role, string securityKey, DateTime expiration)
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

        var key = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(securityKey));
        var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var token = new JwtSecurityToken(
            null, // Thay thế bằng nhà phát hành của bạn
            null, // Thay thế bằng đối tượng của bạn
            claims,
            expires: expiration,
            signingCredentials: credentials
        );
        return new JwtSecurityTokenHandler().WriteToken(token);
    }
}