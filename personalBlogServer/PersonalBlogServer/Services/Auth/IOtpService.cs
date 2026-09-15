namespace PersonalBlogServer.Services.Auth;

public interface IOtpService
{
    string GenerateOtp(string email);
    bool ValidateOtp(string email, string otp);
}
