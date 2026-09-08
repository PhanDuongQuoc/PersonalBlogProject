using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using PersonalBlogServer.Contracts.Public;
using PersonalBlogServer.Models;

namespace PersonalBlogServer.Services.Public;

public sealed class PublicPostService : IPublicPostService
{
    private readonly AppDbContext _context;

    public PublicPostService(AppDbContext context)
    {
        _context = context;
    }

    public async Task<PublicPostDetailResponse?> GetPostBySlugAsync(string slug, CancellationToken cancellationToken = default)
    {
        var post = await _context.Posts
            .Include(p => p.Category)
            .Include(p => p.Author)
            .Include(p => p.Tags)
            .Include(p => p.Comments.OrderByDescending(c => c.CreatedAt))
            .FirstOrDefaultAsync(p => p.Slug == slug && p.Status == "Published", cancellationToken);

        if (post is null)
        {
            return null;
        }

        // Tăng view count trong DB
        try
        {
            post.ViewCount += 1;
            await _context.SaveChangesAsync(cancellationToken);
        }
        catch
        {
            // Bỏ qua lỗi tăng view count nếu có tranh chấp dữ liệu nhỏ
        }

        // Lấy bài viết liên quan
        var relatedPosts = await _context.Posts
            .AsNoTracking()
            .Where(p => p.Status == "Published" && p.Id != post.Id)
            .OrderByDescending(p => p.CategoryId == post.CategoryId)
            .ThenByDescending(p => p.PublishedAt)
            .Take(3)
            .Select(p => new PublicPostSummaryDto(
                p.Id,
                p.Title,
                p.Slug,
                p.Excerpt,
                p.ThumbnailUrl,
                p.Category.Name,
                p.PublishedAt,
                p.ViewCount))
            .ToListAsync(cancellationToken);

        var authorDto = new PublicAuthorDetailDto(
            post.Author.Id,
            post.Author.Username,
            post.Author.DisplayName ?? post.Author.Username,
            post.Author.Role,
            post.Author.AvatarUrl,
            post.Author.Bio,
            post.Author.JobTitle);

        var categoryDto = new PublicCategoryDetailDto(
            post.Category.Id,
            post.Category.Name,
            post.Category.Slug);

        var tagsDto = post.Tags
            .Select(t => new PublicTagDetailDto(t.Id, t.Name, t.Slug))
            .ToList();

        var commentsDto = post.Comments
            .Where(c => c.Status != "Rejected")
            .Select(c => new PublicCommentDetailDto(
                c.Id,
                c.GuestName ?? c.User?.DisplayName ?? "Khách",
                c.Content,
                c.CreatedAt))
            .ToList();

        return new PublicPostDetailResponse(
            post.Id,
            post.Title,
            post.Slug,
            post.Content,
            post.Excerpt,
            post.ThumbnailUrl,
            post.PublishedAt,
            post.ViewCount,
            categoryDto,
            authorDto,
            tagsDto,
            commentsDto,
            relatedPosts);
    }

    public async Task<PublicCommentDetailDto?> AddCommentAsync(string slug, CreateCommentRequest request, CancellationToken cancellationToken = default)
    {
        var post = await _context.Posts
            .FirstOrDefaultAsync(p => p.Slug == slug && p.Status == "Published", cancellationToken);

        if (post is null)
        {
            return null;
        }

        var comment = new Comment
        {
            PostId = post.Id,
            GuestName = string.IsNullOrWhiteSpace(request.Name) ? "Khách" : request.Name.Trim(),
            GuestEmail = string.IsNullOrWhiteSpace(request.Email) ? null : request.Email.Trim(),
            Content = request.Content.Trim(),
            Status = "Pending",
            CreatedAt = DateTime.UtcNow
        };

        _context.Comments.Add(comment);
        await _context.SaveChangesAsync(cancellationToken);

        return new PublicCommentDetailDto(
            comment.Id,
            comment.GuestName,
            comment.Content,
            comment.CreatedAt);
    }
}
