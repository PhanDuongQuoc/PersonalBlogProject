using System.ComponentModel.DataAnnotations;

namespace PersonalBlogServer.Contracts.Auth;

public sealed record LoginRequest(
    [Required(ErrorMessage = "Username hoặc Email là bắt buộc.")]
    string UsernameOrEmail,

    [Required(ErrorMessage = "Mật khẩu là bắt buộc.")]
    string Password
);

public sealed record AuthUserDto(
    int Id,
    string Username,
    string Email,
    string? DisplayName,
    string Role,
    string? AvatarUrl,
    string? JobTitle
);

public sealed record LoginResponse(
    string Token,
    DateTime ExpiresAt,
    AuthUserDto User
);

public sealed record ForgotPasswordRequest(
    [Required(ErrorMessage = "Email là bắt buộc.")]
    [EmailAddress(ErrorMessage = "Email không hợp lệ.")]
    string Email
);

public sealed record ResetPasswordRequest(
    [Required(ErrorMessage = "Email là bắt buộc.")]
    [EmailAddress(ErrorMessage = "Email không hợp lệ.")]
    string Email,

    [Required(ErrorMessage = "Mã xác nhận OTP là bắt buộc.")]
    [StringLength(6, MinimumLength = 6, ErrorMessage = "Mã OTP gồm 6 chữ số.")]
    string Otp,

    [Required(ErrorMessage = "Mật khẩu mới là bắt buộc.")]
    [MinLength(6, ErrorMessage = "Mật khẩu mới tối thiểu 6 ký tự.")]
    string NewPassword
);
