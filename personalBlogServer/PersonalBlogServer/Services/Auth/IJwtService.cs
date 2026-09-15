using PersonalBlogServer.Models;

namespace PersonalBlogServer.Services.Auth;

public interface IJwtService
{
    (string Token, DateTime ExpiresAt) GenerateToken(User user);
    bool VerifyPassword(string inputPassword, string storedPasswordHash);
    string HashPassword(string password);
}
