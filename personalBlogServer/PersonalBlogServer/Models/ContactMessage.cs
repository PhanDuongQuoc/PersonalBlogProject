using System;
using System.Collections.Generic;

namespace PersonalBlogServer.Models;

public partial class ContactMessage
{
    public int Id { get; set; }

    public string Name { get; set; } = null!;

    public string Email { get; set; } = null!;

    public string? Subject { get; set; }

    public string Message { get; set; } = null!;

    public string Status { get; set; } = null!;

    public string? IpAddress { get; set; }

    public string? ReplyNote { get; set; }

    public DateTime? RepliedAt { get; set; }

    public DateTime? CreatedAt { get; set; }

    public DateTime? UpdatedAt { get; set; }
}
