using System;
using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PersonalBlogServer.Contracts.Admin;
using PersonalBlogServer.Models;
using PersonalBlogServer.Services.Auth;

namespace PersonalBlogServer.Controllers;

[ApiController]
[Route("api/admin/security")]
[Authorize]
public class AdminSecurityController : ControllerBase
{
    private readonly AppDbContext _context;
    private readonly IJwtService _jwtService;
    private readonly ILogger<AdminSecurityController> _logger;

    public AdminSecurityController(
        AppDbContext context,
        IJwtService jwtService,
        ILogger<AdminSecurityController> logger)
    {
        _context = context;
        _jwtService = jwtService;
        _logger = logger;
    }

    private int GetCurrentUserId()
    {
        var idClaim = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (int.TryParse(idClaim, out var userId))
        {
            return userId;
        }
        return 1;
    }

    [HttpGet("account-info")]
    public async Task<IActionResult> GetAccountInfo(CancellationToken cancellationToken = default)
    {
        var userId = GetCurrentUserId();
        var user = await _context.Users
            .AsNoTracking()
            .FirstOrDefaultAsync(u => u.Id == userId, cancellationToken);

        if (user == null)
        {
            return NotFound(new { message = "Không tìm thấy thông tin tài khoản." });
        }

        var ip = HttpContext.Connection.RemoteIpAddress?.ToString();
        if (string.IsNullOrEmpty(ip) || ip == "::1")
        {
            ip = "127.0.0.1 (Localhost)";
        }

        var userAgent = Request.Headers["User-Agent"].ToString();
        var browser = "Trình duyệt Web";
        if (userAgent.Contains("Edg")) browser = "Microsoft Edge";
        else if (userAgent.Contains("Chrome")) browser = "Google Chrome";
        else if (userAgent.Contains("Firefox")) browser = "Mozilla Firefox";
        else if (userAgent.Contains("Safari") && !userAgent.Contains("Chrome")) browser = "Apple Safari";

        var dto = new AdminAccountInfoDto
        {
            Id = user.Id,
            Username = user.Username,
            Email = user.Email,
            DisplayName = user.DisplayName ?? user.Username,
            Role = user.Role,
            AvatarUrl = user.AvatarUrl,
            JobTitle = user.JobTitle,
            IsActive = user.IsActive,
            CreatedAt = user.CreatedAt,
            UpdatedAt = user.UpdatedAt,
            CurrentIp = ip,
            CurrentBrowser = browser
        };

        return Ok(dto);
    }

    [HttpPost("change-password")]
    public async Task<IActionResult> ChangePassword([FromBody] ChangePasswordRequest request, CancellationToken cancellationToken = default)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        if (request.NewPassword != request.ConfirmPassword)
        {
            return BadRequest(new { message = "Mật khẩu xác nhận không khớp với mật khẩu mới." });
        }

        if (request.NewPassword.Length < 6)
        {
            return BadRequest(new { message = "Mật khẩu mới phải có tối thiểu 6 ký tự." });
        }

        var userId = GetCurrentUserId();
        var user = await _context.Users
            .FirstOrDefaultAsync(u => u.Id == userId, cancellationToken);

        if (user == null)
        {
            return NotFound(new { message = "Không tìm thấy tài khoản người dùng." });
        }

        // Verify old password with BCrypt
        var isOldPasswordValid = _jwtService.VerifyPassword(request.CurrentPassword, user.PasswordHash);
        if (!isOldPasswordValid)
        {
            _logger.LogWarning("Change password failed: Invalid current password for user {Username}.", user.Username);
            return BadRequest(new { message = "Mật khẩu hiện tại không chính xác. Vui lòng kiểm tra lại!" });
        }

        if (request.CurrentPassword == request.NewPassword)
        {
            return BadRequest(new { message = "Mật khẩu mới không được trùng với mật khẩu hiện tại." });
        }

        // Hash new password and update timestamp
        user.PasswordHash = _jwtService.HashPassword(request.NewPassword.Trim());
        user.UpdatedAt = DateTime.UtcNow;

        await _context.SaveChangesAsync(cancellationToken);

        _logger.LogInformation("Password changed successfully for user {Username}", user.Username);

        return Ok(new ChangePasswordResponse
        {
            Success = true,
            Message = "Đổi mật khẩu thành công! Mật khẩu mới đã được cập nhật an toàn."
        });
    }
}
