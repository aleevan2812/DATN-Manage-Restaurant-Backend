using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.IdentityModel.Tokens;

namespace Application.Services;

public class TokenService
{
    public static string GenerateAccessToken(int userId, string role)
    {
        var claims = new[]
        {
            new Claim("userId", userId.ToString()),
            new Claim("role", role),
            new Claim("tokenType", "AccessToken"),
            new Claim(JwtRegisteredClaimNames.Exp,
                new DateTimeOffset(DateTime.UtcNow.AddDays(7)).ToUnixTimeSeconds().ToString()),
            new Claim(JwtRegisteredClaimNames.Iat,
                new DateTimeOffset(DateTime.UtcNow.AddDays(7)).ToUnixTimeSeconds().ToString())
        };

        var key = new SymmetricSecurityKey(Encoding.ASCII.GetBytes("hoc_lap_trinh_edu_duthanhduoc_com_access"));
        var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var token = new JwtSecurityToken(
            "your_issuer", // Thay thế bằng nhà phát hành của bạn
            "your_audience", // Thay thế bằng đối tượng của bạn
            claims,
            expires: DateTime.UtcNow.AddDays(7),
            signingCredentials: credentials
        );
        return new JwtSecurityTokenHandler().WriteToken(token);
    }

    public static string GenerateRefreshToken(int userId, string role)
    {
        var claims = new[]
        {
            new Claim("userId", userId.ToString()),
            new Claim("role", role),
            new Claim("tokenType", "RefreshToken"),
            new Claim(JwtRegisteredClaimNames.Exp,
                new DateTimeOffset(DateTime.UtcNow.AddDays(7)).ToUnixTimeSeconds().ToString()),
            new Claim(JwtRegisteredClaimNames.Iat,
                new DateTimeOffset(DateTime.UtcNow.AddDays(7)).ToUnixTimeSeconds().ToString())
        };

        var key = new SymmetricSecurityKey(Convert.FromBase64String("hoc_lap_trinh_edu_duthanhduoc_com_refresh"));
        var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var token = new JwtSecurityToken(
            "your_issuer", // Thay thế bằng nhà phát hành của bạn
            "your_audience", // Thay thế bằng đối tượng của bạn
            claims,
            expires: DateTime.UtcNow.AddDays(7),
            signingCredentials: credentials
        );
        return new JwtSecurityTokenHandler().WriteToken(token);
    }
}