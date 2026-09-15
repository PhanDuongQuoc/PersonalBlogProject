using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using PersonalBlogServer.Contracts.Admin;
using PersonalBlogServer.Models;
using PersonalBlogServer.Utils;

namespace PersonalBlogServer.Services.Admin;

public sealed class AdminPostService : IAdminPostService
{
    private readonly AppDbContext _context;

    public AdminPostService(AppDbContext context)
    {
        _context = context;
    }

    public async Task<PagedResult<AdminPostSummaryDto>> GetPostsAsync(
        string? search,
        int? categoryId,
        string? status,
        int page,
        int pageSize,
        CancellationToken cancellationToken = default)
    {
        if (page < 1) page = 1;
        if (pageSize < 1 || pageSize > 100) pageSize = 10;

        var query = _context.Posts
            .AsNoTracking()
            .Include(p => p.Category)
            .Include(p => p.Author)
            .Include(p => p.Tags)
            .AsQueryable();

        // 1. Filter by Search keyword
        if (!string.IsNullOrWhiteSpace(search))
        {
            var s = search.Trim().ToLower();
            query = query.Where(p =>
                p.Title.ToLower().Contains(s) ||
                p.Slug.ToLower().Contains(s) ||
                (p.Excerpt != null && p.Excerpt.ToLower().Contains(s)));
        }

        // 2. Filter by Category
        if (categoryId.HasValue && categoryId.Value > 0)
        {
            query = query.Where(p => p.CategoryId == categoryId.Value);
        }

        // 3. Filter by Status (Published, Draft, Archived)
        if (!string.IsNullOrWhiteSpace(status) && !string.Equals(status, "all", StringComparison.OrdinalIgnoreCase))
        {
            query = query.Where(p => p.Status.ToLower() == status.Trim().ToLower());
        }

        var totalCount = await query.CountAsync(cancellationToken);
        var totalPages = (int)Math.Ceiling(totalCount / (double)pageSize);

        var items = await query
            .OrderByDescending(p => p.CreatedAt)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .Select(p => new AdminPostSummaryDto(
                p.Id,
                p.Title,
                p.Slug,
                p.Excerpt,
                p.ThumbnailUrl,
                p.Status,
                p.CategoryId,
                p.Category != null ? p.Category.Name : "Chưa phân loại",
                p.AuthorId,
                p.Author != null ? (p.Author.DisplayName ?? p.Author.Username) : "Tác giả",
                p.ViewCount,
                p.PublishedAt,
                p.CreatedAt,
                p.UpdatedAt,
                p.Tags != null ? p.Tags.Select(t => t.Name).ToList() : new List<string>()
            ))
            .ToListAsync(cancellationToken);

        return new PagedResult<AdminPostSummaryDto>(items, totalCount, page, pageSize, totalPages);
    }

    public async Task<AdminPostDetailDto?> GetPostByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        var post = await _context.Posts
            .AsNoTracking()
            .Include(p => p.Category)
            .Include(p => p.Author)
            .Include(p => p.Tags)
            .FirstOrDefaultAsync(p => p.Id == id, cancellationToken);

        if (post is null)
        {
            return null;
        }

        return new AdminPostDetailDto(
            post.Id,
            post.Title,
            post.Slug,
            post.Content,
            post.Excerpt,
            post.ThumbnailUrl,
            post.Status,
            new AdminCategorySummaryDto(
                post.Category != null ? post.Category.Id : 0,
                post.Category != null ? post.Category.Name : "Chưa phân loại",
                post.Category != null ? post.Category.Slug : ""
            ),
            post.AuthorId,
            post.Author != null ? (post.Author.DisplayName ?? post.Author.Username) : "Tác giả",
            post.ViewCount,
            post.PublishedAt,
            post.CreatedAt,
            post.UpdatedAt,
            post.Tags != null ? post.Tags.Select(t => new AdminTagSummaryDto(t.Id, t.Name, t.Slug)).ToList() : new List<AdminTagSummaryDto>()
        );
    }

    public async Task<AdminPostDetailDto> CreatePostAsync(int authorId, CreatePostRequest request, CancellationToken cancellationToken = default)
    {
        // 1. Generate unique slug
        var baseSlug = !string.IsNullOrWhiteSpace(request.Slug)
            ? SlugHelper.GenerateSlug(request.Slug)
            : SlugHelper.GenerateSlug(request.Title);

        var uniqueSlug = await EnsureUniqueSlugAsync(baseSlug, null, cancellationToken);

        // 2. Prepare status and published date
        var status = string.IsNullOrWhiteSpace(request.Status) ? "Draft" : request.Status.Trim();
        DateTime? publishedAt = null;
        if (string.Equals(status, "Published", StringComparison.OrdinalIgnoreCase))
        {
            publishedAt = DateTime.UtcNow;
        }

        var post = new Post
        {
            Title = request.Title.Trim(),
            Slug = uniqueSlug,
            Content = request.Content ?? string.Empty,
            Excerpt = string.IsNullOrWhiteSpace(request.Excerpt) ? null : request.Excerpt.Trim(),
            ThumbnailUrl = string.IsNullOrWhiteSpace(request.ThumbnailUrl) ? null : request.ThumbnailUrl.Trim(),
            Status = status,
            AuthorId = authorId,
            CategoryId = request.CategoryId,
            ViewCount = 0,
            PublishedAt = publishedAt,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = null
        };

        // 3. Attach Tags
        if (request.TagNames != null && request.TagNames.Count > 0)
        {
            var tags = await ResolveTagsAsync(request.TagNames, cancellationToken);
            foreach (var tag in tags)
            {
                post.Tags.Add(tag);
            }
        }

        _context.Posts.Add(post);
        await _context.SaveChangesAsync(cancellationToken);

        // Return details
        return (await GetPostByIdAsync(post.Id, cancellationToken))!;
    }

    public async Task<AdminPostDetailDto?> UpdatePostAsync(int id, UpdatePostRequest request, CancellationToken cancellationToken = default)
    {
        var post = await _context.Posts
            .Include(p => p.Tags)
            .FirstOrDefaultAsync(p => p.Id == id, cancellationToken);

        if (post is null)
        {
            return null;
        }

        // 1. Slug check
        var baseSlug = !string.IsNullOrWhiteSpace(request.Slug)
            ? SlugHelper.GenerateSlug(request.Slug)
            : SlugHelper.GenerateSlug(request.Title);

        if (baseSlug != post.Slug)
        {
            post.Slug = await EnsureUniqueSlugAsync(baseSlug, id, cancellationToken);
        }

        // 2. Status change check
        var newStatus = string.IsNullOrWhiteSpace(request.Status) ? post.Status : request.Status.Trim();
        if (string.Equals(newStatus, "Published", StringComparison.OrdinalIgnoreCase) && post.PublishedAt == null)
        {
            post.PublishedAt = DateTime.UtcNow;
        }
        post.Status = newStatus;

        // 3. Properties
        post.Title = request.Title.Trim();
        post.Content = request.Content ?? string.Empty;
        post.Excerpt = string.IsNullOrWhiteSpace(request.Excerpt) ? null : request.Excerpt.Trim();
        post.ThumbnailUrl = string.IsNullOrWhiteSpace(request.ThumbnailUrl) ? null : request.ThumbnailUrl.Trim();
        post.CategoryId = request.CategoryId;
        post.UpdatedAt = DateTime.UtcNow;

        // 4. Tags update
        post.Tags.Clear();
        if (request.TagNames != null && request.TagNames.Count > 0)
        {
            var tags = await ResolveTagsAsync(request.TagNames, cancellationToken);
            foreach (var tag in tags)
            {
                post.Tags.Add(tag);
            }
        }

        await _context.SaveChangesAsync(cancellationToken);

        return (await GetPostByIdAsync(post.Id, cancellationToken))!;
    }

    public async Task<bool> UpdateStatusAsync(int id, string status, CancellationToken cancellationToken = default)
    {
        var post = await _context.Posts.FirstOrDefaultAsync(p => p.Id == id, cancellationToken);
        if (post is null)
        {
            return false;
        }

        post.Status = status;
        if (string.Equals(status, "Published", StringComparison.OrdinalIgnoreCase) && post.PublishedAt == null)
        {
            post.PublishedAt = DateTime.UtcNow;
        }
        post.UpdatedAt = DateTime.UtcNow;

        await _context.SaveChangesAsync(cancellationToken);
        return true;
    }

    public async Task<bool> DeletePostAsync(int id, CancellationToken cancellationToken = default)
    {
        var post = await _context.Posts
            .Include(p => p.Comments)
            .Include(p => p.PostImages)
            .Include(p => p.Tags)
            .FirstOrDefaultAsync(p => p.Id == id, cancellationToken);

        if (post is null)
        {
            return false;
        }

        // Clean up linked comments & images
        if (post.Comments.Count > 0)
        {
            _context.Comments.RemoveRange(post.Comments);
        }
        if (post.PostImages.Count > 0)
        {
            _context.PostImages.RemoveRange(post.PostImages);
        }

        post.Tags.Clear();
        _context.Posts.Remove(post);
        await _context.SaveChangesAsync(cancellationToken);
        return true;
    }

    public async Task<AdminPostStatsDto> GetStatsAsync(CancellationToken cancellationToken = default)
    {
        var totalPosts = await _context.Posts.CountAsync(cancellationToken);
        var publishedPosts = await _context.Posts.CountAsync(p => p.Status == "Published", cancellationToken);
        var draftPosts = await _context.Posts.CountAsync(p => p.Status == "Draft", cancellationToken);
        var totalViews = await _context.Posts.SumAsync(p => (long)p.ViewCount, cancellationToken);

        return new AdminPostStatsDto(totalPosts, publishedPosts, draftPosts, totalViews);
    }

    public async Task<List<AdminCategorySummaryDto>> GetCategoriesAsync(CancellationToken cancellationToken = default)
    {
        return await _context.Categories
            .AsNoTracking()
            .OrderBy(c => c.Name)
            .Select(c => new AdminCategorySummaryDto(c.Id, c.Name, c.Slug))
            .ToListAsync(cancellationToken);
    }

    public async Task<List<AdminTagSummaryDto>> GetTagsAsync(CancellationToken cancellationToken = default)
    {
        return await _context.Tags
            .AsNoTracking()
            .OrderBy(t => t.Name)
            .Select(t => new AdminTagSummaryDto(t.Id, t.Name, t.Slug))
            .ToListAsync(cancellationToken);
    }

    private async Task<string> EnsureUniqueSlugAsync(string baseSlug, int? excludeId, CancellationToken cancellationToken)
    {
        var slug = baseSlug;
        var counter = 1;

        while (true)
        {
            var exists = await _context.Posts
                .AnyAsync(p => p.Slug == slug && (!excludeId.HasValue || p.Id != excludeId.Value), cancellationToken);

            if (!exists)
            {
                return slug;
            }

            slug = $"{baseSlug}-{counter}";
            counter++;
        }
    }

    private async Task<List<Tag>> ResolveTagsAsync(List<string> tagNames, CancellationToken cancellationToken)
    {
        var result = new List<Tag>();
        var cleanNames = tagNames
            .Where(n => !string.IsNullOrWhiteSpace(n))
            .Select(n => n.Trim())
            .Distinct(StringComparer.OrdinalIgnoreCase)
            .ToList();

        foreach (var name in cleanNames)
        {
            var slug = SlugHelper.GenerateSlug(name);
            var existingTag = await _context.Tags
                .FirstOrDefaultAsync(t => t.Name.ToLower() == name.ToLower() || t.Slug == slug, cancellationToken);

            if (existingTag != null)
            {
                result.Add(existingTag);
            }
            else
            {
                var newTag = new Tag
                {
                    Name = name,
                    Slug = slug
                };
                _context.Tags.Add(newTag);
                result.Add(newTag);
            }
        }

        return result;
    }
}
