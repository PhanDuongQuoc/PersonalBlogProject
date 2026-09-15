using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PersonalBlogServer.Contracts.Auth;
using PersonalBlogServer.Models;
using PersonalBlogServer.Services.Auth;

namespace PersonalBlogServer.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly AppDbContext _context;
    private readonly IJwtService _jwtService;
    private readonly IOtpService _otpService;
    private readonly ILogger<AuthController> _logger;

    public AuthController(
        AppDbContext context,
        IJwtService jwtService,
        IOtpService otpService,
        ILogger<AuthController> logger)
    {
        _context = context;
        _jwtService = jwtService;
        _otpService = otpService;
        _logger = logger;
    }

    [HttpPost("login")]
    [AllowAnonymous]
    public async Task<IActionResult> Login([FromBody] LoginRequest request, CancellationToken cancellationToken)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        var normalizedIdentifier = request.UsernameOrEmail.Trim().ToLower();

        var user = await _context.Users
            .AsNoTracking()
            .FirstOrDefaultAsync(u =>
                u.Username.ToLower() == normalizedIdentifier ||
                u.Email.ToLower() == normalizedIdentifier,
                cancellationToken);

        if (user == null)
        {
            _logger.LogWarning("Login failed: User {Identifier} not found.", request.UsernameOrEmail);
            return Unauthorized(new { message = "Tên đăng nhập hoặc mật khẩu không chính xác." });
        }

        if (!user.IsActive)
        {
            _logger.LogWarning("Login failed: User {Username} is deactivated.", user.Username);
            return StatusCode(StatusCodes.Status403Forbidden, new { message = "Tài khoản của bạn đã bị tạm khóa. Vui lòng liên hệ quản trị viên." });
        }

        var isValidPassword = _jwtService.VerifyPassword(request.Password, user.PasswordHash);
        if (!isValidPassword)
        {
            _logger.LogWarning("Login failed: Invalid password for user {Username}.", user.Username);
            return Unauthorized(new { message = "Tên đăng nhập hoặc mật khẩu không chính xác." });
        }

        var (token, expiresAt) = _jwtService.GenerateToken(user);

        var userDto = new AuthUserDto(
            user.Id,
            user.Username,
            user.Email,
            user.DisplayName ?? user.Username,
            user.Role,
            user.AvatarUrl,
            user.JobTitle
        );

        _logger.LogInformation("User {Username} logged in successfully.", user.Username);

        return Ok(new LoginResponse(token, expiresAt, userDto));
    }

    [HttpPost("forgot-password")]
    [AllowAnonymous]
    public async Task<IActionResult> ForgotPassword([FromBody] ForgotPasswordRequest request, CancellationToken cancellationToken)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        var normalizedEmail = request.Email.Trim().ToLower();

        var user = await _context.Users
            .AsNoTracking()
            .FirstOrDefaultAsync(u => u.Email.ToLower() == normalizedEmail, cancellationToken);

        if (user == null)
        {
            return NotFound(new { message = "Không tìm thấy tài khoản tương ứng với email này." });
        }

        if (!user.IsActive)
        {
            return StatusCode(StatusCodes.Status403Forbidden, new { message = "Tài khoản đã bị tạm khóa. Vui lòng liên hệ quản trị viên." });
        }

        var otp = _otpService.GenerateOtp(normalizedEmail);
        _logger.LogInformation("OTP generated for {Email}: {Otp}", normalizedEmail, otp);

        // In dev / test environment, return OTP in response for easy testing
        return Ok(new
        {
            message = "Mã xác nhận OTP (6 chữ số) đã được tạo thành công.",
            otp = otp,
            email = normalizedEmail
        });
    }

    [HttpPost("reset-password")]
    [AllowAnonymous]
    public async Task<IActionResult> ResetPassword([FromBody] ResetPasswordRequest request, CancellationToken cancellationToken)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        var normalizedEmail = request.Email.Trim().ToLower();

        var isOtpValid = _otpService.ValidateOtp(normalizedEmail, request.Otp);
        if (!isOtpValid)
        {
            return BadRequest(new { message = "Mã OTP không chính xác hoặc đã hết hạn. Vui lòng thử lại!" });
        }

        var user = await _context.Users
            .FirstOrDefaultAsync(u => u.Email.ToLower() == normalizedEmail, cancellationToken);

        if (user == null)
        {
            return NotFound(new { message = "Không tìm thấy tài khoản tương ứng với email này." });
        }

        if (!user.IsActive)
        {
            return StatusCode(StatusCodes.Status403Forbidden, new { message = "Tài khoản đã bị tạm khóa." });
        }

        // Hash and update the new password (also save hashed with BCrypt)
        user.PasswordHash = _jwtService.HashPassword(request.NewPassword.Trim());
        user.UpdatedAt = DateTime.UtcNow;

        await _context.SaveChangesAsync(cancellationToken);

        _logger.LogInformation("Password reset successfully for user {Username} ({Email})", user.Username, user.Email);

        return Ok(new { message = "Đặt lại mật khẩu thành công! Bạn có thể đăng nhập ngay bây giờ." });
    }

    [HttpGet("me")]
    [Authorize]
    public async Task<IActionResult> GetCurrentUser(CancellationToken cancellationToken)
    {
        var userIdClaim = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (string.IsNullOrEmpty(userIdClaim) || !int.TryParse(userIdClaim, out var userId))
        {
            return Unauthorized(new { message = "Phiên đăng nhập không hợp lệ hoặc đã hết hạn." });
        }

        var user = await _context.Users
            .AsNoTracking()
            .FirstOrDefaultAsync(u => u.Id == userId, cancellationToken);

        if (user == null || !user.IsActive)
        {
            return Unauthorized(new { message = "Người dùng không tồn tại hoặc đã bị vô hiệu hóa." });
        }

        var userDto = new AuthUserDto(
            user.Id,
            user.Username,
            user.Email,
            user.DisplayName ?? user.Username,
            user.Role,
            user.AvatarUrl,
            user.JobTitle
        );

        return Ok(userDto);
    }
}
