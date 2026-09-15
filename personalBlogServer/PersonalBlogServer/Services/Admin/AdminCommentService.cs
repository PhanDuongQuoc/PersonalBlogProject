using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using PersonalBlogServer.Contracts.Admin;
using PersonalBlogServer.Models;

namespace PersonalBlogServer.Services.Admin;

public class AdminCommentService : IAdminCommentService
{
    private readonly AppDbContext _context;

    public AdminCommentService(AppDbContext context)
    {
        _context = context;
    }

    public async Task<PagedResult<AdminCommentListDto>> GetCommentsAsync(
        string? search,
        string? status,
        int? postId,
        int page,
        int pageSize,
        CancellationToken cancellationToken = default)
    {
        page = Math.Max(1, page);
        pageSize = Math.Clamp(pageSize, 1, 100);

        var query = _context.Comments
            .AsNoTracking()
            .Include(c => c.Post)
            .Include(c => c.User)
            .AsQueryable();

        if (postId.HasValue && postId.Value > 0)
        {
            query = query.Where(c => c.PostId == postId.Value);
        }

        if (!string.IsNullOrWhiteSpace(status) && !string.Equals(status, "all", StringComparison.OrdinalIgnoreCase))
        {
            query = query.Where(c => c.Status == status);
        }

        if (!string.IsNullOrWhiteSpace(search))
        {
            var term = search.Trim();
            query = query.Where(c =>
                EF.Functions.ILike(c.Content, $"%{term}%") ||
                (c.GuestName != null && EF.Functions.ILike(c.GuestName, $"%{term}%")) ||
                (c.GuestEmail != null && EF.Functions.ILike(c.GuestEmail, $"%{term}%")) ||
                (c.User != null && c.User.DisplayName != null && EF.Functions.ILike(c.User.DisplayName, $"%{term}%")) ||
                (c.User != null && EF.Functions.ILike(c.User.Username, $"%{term}%")) ||
                EF.Functions.ILike(c.Post.Title, $"%{term}%")
            );
        }

        var totalCount = await query.CountAsync(cancellationToken);
        var totalPages = (int)Math.Ceiling(totalCount / (double)pageSize);

        var items = await query
            .OrderByDescending(c => c.CreatedAt)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .Select(c => new AdminCommentListDto
            {
                Id = c.Id,
                PostId = c.PostId,
                PostTitle = c.Post.Title,
                PostSlug = c.Post.Slug,
                UserId = c.UserId,
                AuthorName = c.UserId.HasValue && c.User != null
                    ? (c.User.DisplayName ?? c.User.Username)
                    : (c.GuestName ?? "Khách độc giả"),
                AuthorEmail = c.UserId.HasValue && c.User != null
                    ? c.User.Email
                    : c.GuestEmail,
                AuthorAvatar = c.UserId.HasValue && c.User != null
                    ? c.User.AvatarUrl
                    : null,
                Content = c.Content,
                Status = c.Status,
                CreatedAt = c.CreatedAt,
                UpdatedAt = c.UpdatedAt
            })
            .ToListAsync(cancellationToken);

        return new PagedResult<AdminCommentListDto>(
            items,
            totalCount,
            page,
            pageSize,
            totalPages
        );
    }

    public async Task<AdminCommentStatsDto> GetStatsAsync(CancellationToken cancellationToken = default)
    {
        var totalComments = await _context.Comments.CountAsync(cancellationToken);
        var approvedComments = await _context.Comments.CountAsync(c => c.Status == "Approved", cancellationToken);
        var pendingComments = await _context.Comments.CountAsync(c => c.Status == "Pending", cancellationToken);
        var spamComments = await _context.Comments.CountAsync(c => c.Status == "Spam" || c.Status == "Rejected", cancellationToken);

        return new AdminCommentStatsDto
        {
            TotalComments = totalComments,
            ApprovedComments = approvedComments,
            PendingComments = pendingComments,
            SpamComments = spamComments
        };
    }

    public async Task<AdminCommentDetailDto?> GetCommentByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        var comment = await _context.Comments
            .AsNoTracking()
            .Include(c => c.Post)
            .Include(c => c.User)
            .Where(c => c.Id == id)
            .Select(c => new AdminCommentDetailDto
            {
                Id = c.Id,
                PostId = c.PostId,
                PostTitle = c.Post.Title,
                PostSlug = c.Post.Slug,
                UserId = c.UserId,
                AuthorName = c.UserId.HasValue && c.User != null
                    ? (c.User.DisplayName ?? c.User.Username)
                    : (c.GuestName ?? "Khách độc giả"),
                AuthorEmail = c.UserId.HasValue && c.User != null
                    ? c.User.Email
                    : c.GuestEmail,
                AuthorAvatar = c.UserId.HasValue && c.User != null
                    ? c.User.AvatarUrl
                    : null,
                Content = c.Content,
                Status = c.Status,
                CreatedAt = c.CreatedAt,
                UpdatedAt = c.UpdatedAt
            })
            .FirstOrDefaultAsync(cancellationToken);

        return comment;
    }

    public async Task<bool> UpdateStatusAsync(int id, string status, CancellationToken cancellationToken = default)
    {
        var comment = await _context.Comments.FirstOrDefaultAsync(c => c.Id == id, cancellationToken);
        if (comment is null)
        {
            return false;
        }

        comment.Status = status;
        comment.UpdatedAt = DateTime.UtcNow;

        await _context.SaveChangesAsync(cancellationToken);
        return true;
    }

    public async Task<bool> DeleteCommentAsync(int id, CancellationToken cancellationToken = default)
    {
        var comment = await _context.Comments.FirstOrDefaultAsync(c => c.Id == id, cancellationToken);
        if (comment is null)
        {
            return false;
        }

        _context.Comments.Remove(comment);
        await _context.SaveChangesAsync(cancellationToken);
        return true;
    }
}
