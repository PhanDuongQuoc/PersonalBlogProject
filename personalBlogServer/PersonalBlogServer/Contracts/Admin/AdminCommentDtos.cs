using System;

namespace PersonalBlogServer.Contracts.Admin;

public class AdminCommentListDto
{
    public int Id { get; set; }
    public int PostId { get; set; }
    public string PostTitle { get; set; } = null!;
    public string PostSlug { get; set; } = null!;
    public int? UserId { get; set; }
    public string AuthorName { get; set; } = null!;
    public string? AuthorEmail { get; set; }
    public string? AuthorAvatar { get; set; }
    public string Content { get; set; } = null!;
    public string Status { get; set; } = null!;
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
}

public class AdminCommentDetailDto
{
    public int Id { get; set; }
    public int PostId { get; set; }
    public string PostTitle { get; set; } = null!;
    public string PostSlug { get; set; } = null!;
    public int? UserId { get; set; }
    public string AuthorName { get; set; } = null!;
    public string? AuthorEmail { get; set; }
    public string? AuthorAvatar { get; set; }
    public string Content { get; set; } = null!;
    public string Status { get; set; } = null!;
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
}

public class UpdateCommentStatusRequest
{
    public string Status { get; set; } = null!;
}

public class AdminCommentStatsDto
{
    public int TotalComments { get; set; }
    public int ApprovedComments { get; set; }
    public int PendingComments { get; set; }
    public int SpamComments { get; set; }
}
