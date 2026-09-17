using System;
using System.Collections.Generic;

namespace PersonalBlogServer.Contracts.Admin;

public class ContactMessageDto
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string? Subject { get; set; }
    public string Message { get; set; } = string.Empty;
    public string Status { get; set; } = "Unread"; // Unread, Read, Replied, Archived
    public string? IpAddress { get; set; }
    public string? ReplyNote { get; set; }
    public DateTime? RepliedAt { get; set; }
    public DateTime? CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
}

public class ContactListQuery
{
    public int PageIndex { get; set; } = 1;
    public int PageSize { get; set; } = 10;
    public string? Status { get; set; } // All, Unread, Read, Replied, Archived
    public string? Keyword { get; set; }
}

public class ContactListResponse
{
    public int TotalCount { get; set; }
    public int PageIndex { get; set; }
    public int PageSize { get; set; }
    public int TotalPages => (int)Math.Ceiling((double)TotalCount / (PageSize > 0 ? PageSize : 10));
    public int UnreadCount { get; set; }
    public int RepliedCount { get; set; }
    public List<ContactMessageDto> Items { get; set; } = new();
}

public class UpdateContactStatusRequest
{
    public string Status { get; set; } = "Read"; // Unread, Read, Replied, Archived
    public string? ReplyNote { get; set; }
}

public class ContactSummaryDto
{
    public int TotalMessages { get; set; }
    public int UnreadMessages { get; set; }
    public int RepliedMessages { get; set; }
    public int ThisMonthMessages { get; set; }
}
