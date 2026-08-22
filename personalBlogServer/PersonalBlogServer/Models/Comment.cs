using System;
using System.Collections.Generic;

namespace PersonalBlogServer.Models;

public partial class Comment
{
    public int Id { get; set; }

    public int PostId { get; set; }

    public int? UserId { get; set; }

    public string? GuestName { get; set; }

    public string? GuestEmail { get; set; }

    public string Content { get; set; } = null!;

    public string Status { get; set; } = null!;

    public DateTime CreatedAt { get; set; }

    public DateTime? UpdatedAt { get; set; }

    public virtual Post Post { get; set; } = null!;

    public virtual User? User { get; set; }
}
