using System.Collections.Concurrent;
using System.Security.Cryptography;

namespace PersonalBlogServer.Services.Auth;

public class OtpService : IOtpService
{
    private sealed record OtpEntry(string Otp, DateTime ExpiresAt);

    private readonly ConcurrentDictionary<string, OtpEntry> _otpStore = new(StringComparer.OrdinalIgnoreCase);

    public string GenerateOtp(string email)
    {
        var randomBytes = new byte[4];
        using (var rng = RandomNumberGenerator.Create())
        {
            rng.GetBytes(randomBytes);
        }
        var randomInt = Math.Abs(BitConverter.ToInt32(randomBytes, 0));
        var otp = (randomInt % 900000 + 100000).ToString(); // 6-digit OTP (100000 - 999999)

        var expiresAt = DateTime.UtcNow.AddMinutes(10); // Valid for 10 minutes
        _otpStore[email] = new OtpEntry(otp, expiresAt);

        return otp;
    }

    public bool ValidateOtp(string email, string otp)
    {
        if (string.IsNullOrWhiteSpace(email) || string.IsNullOrWhiteSpace(otp))
        {
            return false;
        }

        if (!_otpStore.TryGetValue(email, out var entry))
        {
            return false;
        }

        if (DateTime.UtcNow > entry.ExpiresAt)
        {
            _otpStore.TryRemove(email, out _);
            return false;
        }

        if (entry.Otp == otp.Trim())
        {
            _otpStore.TryRemove(email, out _); // Invalidate after successful verification
            return true;
        }

        return false;
    }
}
