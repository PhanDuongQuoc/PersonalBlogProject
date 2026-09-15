using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.IdentityModel.Tokens;
using PersonalBlogServer.Models;

namespace PersonalBlogServer.Services.Auth;

public class JwtService : IJwtService
{
    private readonly IConfiguration _configuration;

    public JwtService(IConfiguration configuration)
    {
        _configuration = configuration;
    }

    public (string Token, DateTime ExpiresAt) GenerateToken(User user)
    {
        var jwtSettings = _configuration.GetSection("Jwt");
        var secretKey = jwtSettings["Key"] ?? "DefaultSuperSecretKeyForPersonalBlogProject2026";
        var issuer = jwtSettings["Issuer"] ?? "MyAuthServer";
        var audience = jwtSettings["Audience"] ?? "PersonalBlogApp";
        var durationInMinutes = int.TryParse(jwtSettings["DurationInMinutes"], out var d) ? d : 1440; // Default 24h

        var expiresAt = DateTime.UtcNow.AddMinutes(durationInMinutes);

        var claims = new List<Claim>
        {
            new(ClaimTypes.NameIdentifier, user.Id.ToString()),
            new(ClaimTypes.Name, user.Username),
            new(ClaimTypes.Email, user.Email),
            new(ClaimTypes.Role, user.Role ?? "Author"),
            new("DisplayName", user.DisplayName ?? user.Username)
        };

        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey));
        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var tokenDescriptor = new JwtSecurityToken(
            issuer: issuer,
            audience: audience,
            claims: claims,
            expires: expiresAt,
            signingCredentials: creds
        );

        var tokenHandler = new JwtSecurityTokenHandler();
        var token = tokenHandler.WriteToken(tokenDescriptor);

        return (token, expiresAt);
    }

    public bool VerifyPassword(string inputPassword, string storedPasswordHash)
    {
        if (string.IsNullOrWhiteSpace(storedPasswordHash) || string.IsNullOrWhiteSpace(inputPassword))
        {
            return false;
        }

        var input = inputPassword.Trim();
        var stored = storedPasswordHash.Trim();

        // 1. Direct match check (hỗ trợ lưu password dạng plain text như '123456' trong database cho môi trường dev)
        if (input == stored)
        {
            return true;
        }

        // 2. BCrypt verification (nếu lưu dạng mã băm BCrypt $2a$, $2b$, $2y$)
        if (stored.StartsWith("$2"))
        {
            try
            {
                if (BCrypt.Net.BCrypt.Verify(input, stored))
                {
                    return true;
                }
            }
            catch
            {
                // Bỏ qua lỗi format hash không hợp lệ
            }
        }

        return false;
    }

    public string HashPassword(string password)
    {
        return BCrypt.Net.BCrypt.HashPassword(password);
    }
}
