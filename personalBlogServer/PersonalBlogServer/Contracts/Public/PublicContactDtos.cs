using System.ComponentModel.DataAnnotations;

namespace PersonalBlogServer.Contracts.Public;

public class SubmitContactRequest
{
    [Required(ErrorMessage = "Vui lòng nhập họ và tên.")]
    [MaxLength(150, ErrorMessage = "Tên không được vượt quá 150 ký tự.")]
    public string Name { get; set; } = string.Empty;

    [Required(ErrorMessage = "Vui lòng nhập địa chỉ email.")]
    [EmailAddress(ErrorMessage = "Email không đúng định dạng.")]
    [MaxLength(255, ErrorMessage = "Email không được vượt quá 255 ký tự.")]
    public string Email { get; set; } = string.Empty;

    [MaxLength(255, ErrorMessage = "Tiêu đề không được vượt quá 255 ký tự.")]
    public string? Subject { get; set; }

    [Required(ErrorMessage = "Vui lòng nhập nội dung tin nhắn.")]
    [MaxLength(5000, ErrorMessage = "Tin nhắn không được vượt quá 5000 ký tự.")]
    public string Message { get; set; } = string.Empty;

    public bool Botcheck { get; set; } = false;
}

public class SubmitContactResponse
{
    public bool Success { get; set; } = true;
    public string Message { get; set; } = "Cảm ơn bạn! Tin nhắn của bạn đã được gửi thành công.";
    public int? ContactId { get; set; }
}
