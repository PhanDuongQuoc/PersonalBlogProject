using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using PersonalBlogServer.Contracts.Admin;
using PersonalBlogServer.Hubs;
using PersonalBlogServer.Models;

namespace PersonalBlogServer.Services.Admin;

public class AdminContactService : IAdminContactService
{
    private readonly AppDbContext _dbContext;
    private readonly IHubContext<NotificationHub> _hubContext;
    private readonly ILogger<AdminContactService> _logger;

    public AdminContactService(
        AppDbContext dbContext,
        IHubContext<NotificationHub> hubContext,
        ILogger<AdminContactService> logger)
    {
        _dbContext = dbContext;
        _hubContext = hubContext;
        _logger = logger;
    }

    public async Task<ContactListResponse> GetContactsAsync(ContactListQuery query, CancellationToken cancellationToken = default)
    {
        var dbQuery = _dbContext.ContactMessages.AsQueryable();

        // 1. Status Filter
        if (!string.IsNullOrWhiteSpace(query.Status) && !query.Status.Equals("All", StringComparison.OrdinalIgnoreCase))
        {
            dbQuery = dbQuery.Where(c => c.Status.ToLower() == query.Status.ToLower());
        }

        // 2. Keyword Search
        if (!string.IsNullOrWhiteSpace(query.Keyword))
        {
            var kw = query.Keyword.Trim().ToLower();
            dbQuery = dbQuery.Where(c =>
                c.Name.ToLower().Contains(kw) ||
                c.Email.ToLower().Contains(kw) ||
                (c.Subject != null && c.Subject.ToLower().Contains(kw)) ||
                c.Message.ToLower().Contains(kw)
            );
        }

        var totalCount = await dbQuery.CountAsync(cancellationToken);
        var unreadCount = await _dbContext.ContactMessages.CountAsync(c => c.Status == "Unread", cancellationToken);
        var repliedCount = await _dbContext.ContactMessages.CountAsync(c => c.Status == "Replied", cancellationToken);

        var pageIndex = query.PageIndex > 0 ? query.PageIndex : 1;
        var pageSize = query.PageSize > 0 ? query.PageSize : 10;

        var rawItems = await dbQuery
            .OrderByDescending(c => c.CreatedAt)
            .Skip((pageIndex - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(cancellationToken);

        var items = rawItems.Select(MapToDto).ToList();

        return new ContactListResponse
        {
            TotalCount = totalCount,
            PageIndex = pageIndex,
            PageSize = pageSize,
            UnreadCount = unreadCount,
            RepliedCount = repliedCount,
            Items = items
        };
    }

    public async Task<ContactMessageDto?> GetContactByIdAsync(int id, bool autoMarkAsRead = true, CancellationToken cancellationToken = default)
    {
        var contact = await _dbContext.ContactMessages.FirstOrDefaultAsync(c => c.Id == id, cancellationToken);
        if (contact == null) return null;

        if (autoMarkAsRead && contact.Status == "Unread")
        {
            contact.Status = "Read";
            contact.UpdatedAt = DateTime.UtcNow;
            await _dbContext.SaveChangesAsync(cancellationToken);

            // Notify UI about status change
            await NotifyStatusUpdateAsync(contact.Id, contact.Status, cancellationToken);
        }

        return MapToDto(contact);
    }

    public async Task<ContactMessageDto?> UpdateContactStatusAsync(int id, UpdateContactStatusRequest request, CancellationToken cancellationToken = default)
    {
        var contact = await _dbContext.ContactMessages.FirstOrDefaultAsync(c => c.Id == id, cancellationToken);
        if (contact == null) return null;

        contact.Status = request.Status;
        if (!string.IsNullOrWhiteSpace(request.ReplyNote))
        {
            contact.ReplyNote = request.ReplyNote;
        }

        if (request.Status.Equals("Replied", StringComparison.OrdinalIgnoreCase))
        {
            contact.RepliedAt = DateTime.UtcNow;
        }

        contact.UpdatedAt = DateTime.UtcNow;
        await _dbContext.SaveChangesAsync(cancellationToken);

        await NotifyStatusUpdateAsync(contact.Id, contact.Status, cancellationToken);

        return MapToDto(contact);
    }

    public async Task<bool> DeleteContactAsync(int id, CancellationToken cancellationToken = default)
    {
        var contact = await _dbContext.ContactMessages.FirstOrDefaultAsync(c => c.Id == id, cancellationToken);
        if (contact == null) return false;

        _dbContext.ContactMessages.Remove(contact);
        await _dbContext.SaveChangesAsync(cancellationToken);
        return true;
    }

    public async Task<ContactSummaryDto> GetContactSummaryAsync(CancellationToken cancellationToken = default)
    {
        var now = DateTime.UtcNow;
        var startOfMonth = new DateTime(now.Year, now.Month, 1, 0, 0, 0, DateTimeKind.Utc);

        var total = await _dbContext.ContactMessages.CountAsync(cancellationToken);
        var unread = await _dbContext.ContactMessages.CountAsync(c => c.Status == "Unread", cancellationToken);
        var replied = await _dbContext.ContactMessages.CountAsync(c => c.Status == "Replied", cancellationToken);
        var thisMonth = await _dbContext.ContactMessages.CountAsync(c => c.CreatedAt >= startOfMonth, cancellationToken);

        return new ContactSummaryDto
        {
            TotalMessages = total,
            UnreadMessages = unread,
            RepliedMessages = replied,
            ThisMonthMessages = thisMonth
        };
    }

    private static DateTime? AsUtc(DateTime? dt) =>
        dt.HasValue ? DateTime.SpecifyKind(dt.Value, DateTimeKind.Utc) : null;

    private static ContactMessageDto MapToDto(ContactMessage c)
    {
        return new ContactMessageDto
        {
            Id = c.Id,
            Name = c.Name,
            Email = c.Email,
            Subject = c.Subject,
            Message = c.Message,
            Status = c.Status,
            IpAddress = c.IpAddress,
            ReplyNote = c.ReplyNote,
            RepliedAt = AsUtc(c.RepliedAt),
            CreatedAt = AsUtc(c.CreatedAt),
            UpdatedAt = AsUtc(c.UpdatedAt)
        };
    }

    private async Task NotifyStatusUpdateAsync(int id, string status, CancellationToken cancellationToken)
    {
        try
        {
            await _hubContext.Clients.All.SendAsync("ReceiveContactStatusUpdate", new { id, status }, cancellationToken);
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "Failed to send ReceiveContactStatusUpdate signal for ID {Id}", id);
        }
    }
}
