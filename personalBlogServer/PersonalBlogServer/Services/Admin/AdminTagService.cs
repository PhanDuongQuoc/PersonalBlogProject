using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using PersonalBlogServer.Contracts.Admin;
using PersonalBlogServer.Models;
using PersonalBlogServer.Utils;

namespace PersonalBlogServer.Services.Admin;

public class AdminTagService : IAdminTagService
{
    private readonly AppDbContext _context;

    public AdminTagService(AppDbContext context)
    {
        _context = context;
    }

    public async Task<PagedResult<AdminTagListDto>> GetTagsAsync(
        string? search,
        int page,
        int pageSize,
        CancellationToken cancellationToken = default)
    {
        page = Math.Max(1, page);
        pageSize = Math.Clamp(pageSize, 1, 100);

        var query = _context.Tags
            .AsNoTracking()
            .AsQueryable();

        if (!string.IsNullOrWhiteSpace(search))
        {
            var term = search.Trim();
            query = query.Where(t =>
                EF.Functions.ILike(t.Name, $"%{term}%") ||
                EF.Functions.ILike(t.Slug, $"%{term}%")
            );
        }

        var totalCount = await query.CountAsync(cancellationToken);
        var totalPages = (int)Math.Ceiling(totalCount / (double)pageSize);

        var items = await query
            .OrderByDescending(t => t.CreatedAt)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .Select(t => new AdminTagListDto
            {
                Id = t.Id,
                Name = t.Name,
                Slug = t.Slug,
                PostCount = t.Posts.Count,
                CreatedAt = t.CreatedAt
            })
            .ToListAsync(cancellationToken);

        return new PagedResult<AdminTagListDto>(
            items,
            totalCount,
            page,
            pageSize,
            totalPages
        );
    }

    public async Task<AdminTagStatsDto> GetStatsAsync(CancellationToken cancellationToken = default)
    {
        var totalTags = await _context.Tags.CountAsync(cancellationToken);
        var totalAssignedPosts = await _context.Posts.SelectMany(p => p.Tags).CountAsync(cancellationToken);
        var tagsWithNoPosts = await _context.Tags
            .Where(t => !t.Posts.Any())
            .CountAsync(cancellationToken);

        var topTag = await _context.Tags
            .Select(t => new
            {
                t.Name,
                PostCount = t.Posts.Count
            })
            .OrderByDescending(t => t.PostCount)
            .FirstOrDefaultAsync(cancellationToken);

        return new AdminTagStatsDto
        {
            TotalTags = totalTags,
            TotalAssignedPosts = totalAssignedPosts,
            TagsWithNoPosts = tagsWithNoPosts,
            TopTagName = topTag?.Name ?? "Chưa có",
            TopTagPostCount = topTag?.PostCount ?? 0
        };
    }

    public async Task<AdminTagDetailDto?> GetTagByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        var tag = await _context.Tags
            .AsNoTracking()
            .Where(t => t.Id == id)
            .Select(t => new AdminTagDetailDto
            {
                Id = t.Id,
                Name = t.Name,
                Slug = t.Slug,
                PostCount = t.Posts.Count,
                CreatedAt = t.CreatedAt
            })
            .FirstOrDefaultAsync(cancellationToken);

        return tag;
    }

    public async Task<AdminTagDetailDto> CreateTagAsync(CreateTagRequest request, CancellationToken cancellationToken = default)
    {
        var cleanName = request.Name.Trim().Replace("#", "");
        var baseSlug = !string.IsNullOrWhiteSpace(request.Slug)
            ? SlugHelper.GenerateSlug(request.Slug)
            : SlugHelper.GenerateSlug(cleanName);

        var uniqueSlug = await GenerateUniqueSlugAsync(baseSlug, null, cancellationToken);

        var tag = new Tag
        {
            Name = cleanName,
            Slug = uniqueSlug,
            CreatedAt = DateTime.UtcNow
        };

        _context.Tags.Add(tag);
        await _context.SaveChangesAsync(cancellationToken);

        return new AdminTagDetailDto
        {
            Id = tag.Id,
            Name = tag.Name,
            Slug = tag.Slug,
            PostCount = 0,
            CreatedAt = tag.CreatedAt
        };
    }

    public async Task<AdminTagDetailDto?> UpdateTagAsync(int id, UpdateTagRequest request, CancellationToken cancellationToken = default)
    {
        var tag = await _context.Tags.FirstOrDefaultAsync(t => t.Id == id, cancellationToken);
        if (tag is null)
        {
            return null;
        }

        var cleanName = request.Name.Trim().Replace("#", "");
        var baseSlug = !string.IsNullOrWhiteSpace(request.Slug)
            ? SlugHelper.GenerateSlug(request.Slug)
            : SlugHelper.GenerateSlug(cleanName);

        var uniqueSlug = await GenerateUniqueSlugAsync(baseSlug, id, cancellationToken);

        tag.Name = cleanName;
        tag.Slug = uniqueSlug;

        await _context.SaveChangesAsync(cancellationToken);

        var postCount = await _context.Posts.CountAsync(p => p.Tags.Any(t => t.Id == id), cancellationToken);

        return new AdminTagDetailDto
        {
            Id = tag.Id,
            Name = tag.Name,
            Slug = tag.Slug,
            PostCount = postCount,
            CreatedAt = tag.CreatedAt
        };
    }

    public async Task<bool> DeleteTagAsync(int id, CancellationToken cancellationToken = default)
    {
        var tag = await _context.Tags
            .Include(t => t.Posts)
            .FirstOrDefaultAsync(t => t.Id == id, cancellationToken);

        if (tag is null)
        {
            return false;
        }

        // Remove tag associations from post_tags junction table
        tag.Posts.Clear();
        _context.Tags.Remove(tag);
        await _context.SaveChangesAsync(cancellationToken);

        return true;
    }

    private async Task<string> GenerateUniqueSlugAsync(string baseSlug, int? excludeId, CancellationToken cancellationToken)
    {
        var slug = baseSlug;
        var counter = 1;

        while (true)
        {
            var query = _context.Tags.Where(t => t.Slug == slug);
            if (excludeId.HasValue)
            {
                query = query.Where(t => t.Id != excludeId.Value);
            }

            var exists = await query.AnyAsync(cancellationToken);
            if (!exists)
            {
                return slug;
            }

            counter++;
            slug = $"{baseSlug}-{counter}";
        }
    }
}
