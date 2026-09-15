using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using PersonalBlogServer.Contracts.Admin;
using PersonalBlogServer.Models;
using PersonalBlogServer.Utils;

namespace PersonalBlogServer.Services.Admin;

public class AdminCategoryService : IAdminCategoryService
{
    private readonly AppDbContext _context;

    public AdminCategoryService(AppDbContext context)
    {
        _context = context;
    }

    public async Task<PagedResult<AdminCategoryListDto>> GetCategoriesAsync(
        string? search,
        int page,
        int pageSize,
        CancellationToken cancellationToken = default)
    {
        page = Math.Max(1, page);
        pageSize = Math.Clamp(pageSize, 1, 100);

        var query = _context.Categories
            .AsNoTracking()
            .AsQueryable();

        if (!string.IsNullOrWhiteSpace(search))
        {
            var term = search.Trim();
            query = query.Where(c =>
                EF.Functions.ILike(c.Name, $"%{term}%") ||
                EF.Functions.ILike(c.Slug, $"%{term}%") ||
                (c.Description != null && EF.Functions.ILike(c.Description, $"%{term}%"))
            );
        }

        var totalCount = await query.CountAsync(cancellationToken);
        var totalPages = (int)Math.Ceiling(totalCount / (double)pageSize);

        var items = await query
            .OrderByDescending(c => c.CreatedAt)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .Select(c => new AdminCategoryListDto
            {
                Id = c.Id,
                Name = c.Name,
                Slug = c.Slug,
                Description = c.Description,
                ThumbnailUrl = c.ThumbnailUrl,
                PostCount = c.Posts.Count,
                CreatedAt = c.CreatedAt
            })
            .ToListAsync(cancellationToken);

        return new PagedResult<AdminCategoryListDto>(
            items,
            totalCount,
            page,
            pageSize,
            totalPages
        );
    }

    public async Task<AdminCategoryStatsDto> GetStatsAsync(CancellationToken cancellationToken = default)
    {
        var totalCategories = await _context.Categories.CountAsync(cancellationToken);
        var totalAssignedPosts = await _context.Posts.CountAsync(cancellationToken);
        var categoriesWithNoPosts = await _context.Categories
            .Where(c => !c.Posts.Any())
            .CountAsync(cancellationToken);

        var topCategory = await _context.Categories
            .Select(c => new
            {
                c.Name,
                PostCount = c.Posts.Count
            })
            .OrderByDescending(c => c.PostCount)
            .FirstOrDefaultAsync(cancellationToken);

        return new AdminCategoryStatsDto
        {
            TotalCategories = totalCategories,
            TotalAssignedPosts = totalAssignedPosts,
            CategoriesWithNoPosts = categoriesWithNoPosts,
            TopCategoryName = topCategory?.Name ?? "Chưa có",
            TopCategoryPostCount = topCategory?.PostCount ?? 0
        };
    }

    public async Task<AdminCategoryDetailDto?> GetCategoryByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        var category = await _context.Categories
            .AsNoTracking()
            .Where(c => c.Id == id)
            .Select(c => new AdminCategoryDetailDto
            {
                Id = c.Id,
                Name = c.Name,
                Slug = c.Slug,
                Description = c.Description,
                ThumbnailUrl = c.ThumbnailUrl,
                PostCount = c.Posts.Count,
                CreatedAt = c.CreatedAt
            })
            .FirstOrDefaultAsync(cancellationToken);

        return category;
    }

    public async Task<AdminCategoryDetailDto> CreateCategoryAsync(CreateCategoryRequest request, CancellationToken cancellationToken = default)
    {
        var baseSlug = !string.IsNullOrWhiteSpace(request.Slug)
            ? SlugHelper.GenerateSlug(request.Slug)
            : SlugHelper.GenerateSlug(request.Name);

        var uniqueSlug = await GenerateUniqueSlugAsync(baseSlug, null, cancellationToken);

        var category = new Category
        {
            Name = request.Name.Trim(),
            Slug = uniqueSlug,
            Description = string.IsNullOrWhiteSpace(request.Description) ? null : request.Description.Trim(),
            ThumbnailUrl = string.IsNullOrWhiteSpace(request.ThumbnailUrl) ? null : request.ThumbnailUrl.Trim(),
            CreatedAt = DateTime.UtcNow
        };

        _context.Categories.Add(category);
        await _context.SaveChangesAsync(cancellationToken);

        return new AdminCategoryDetailDto
        {
            Id = category.Id,
            Name = category.Name,
            Slug = category.Slug,
            Description = category.Description,
            ThumbnailUrl = category.ThumbnailUrl,
            PostCount = 0,
            CreatedAt = category.CreatedAt
        };
    }

    public async Task<AdminCategoryDetailDto?> UpdateCategoryAsync(int id, UpdateCategoryRequest request, CancellationToken cancellationToken = default)
    {
        var category = await _context.Categories.FirstOrDefaultAsync(c => c.Id == id, cancellationToken);
        if (category is null)
        {
            return null;
        }

        var baseSlug = !string.IsNullOrWhiteSpace(request.Slug)
            ? SlugHelper.GenerateSlug(request.Slug)
            : SlugHelper.GenerateSlug(request.Name);

        var uniqueSlug = await GenerateUniqueSlugAsync(baseSlug, id, cancellationToken);

        category.Name = request.Name.Trim();
        category.Slug = uniqueSlug;
        category.Description = string.IsNullOrWhiteSpace(request.Description) ? null : request.Description.Trim();
        category.ThumbnailUrl = string.IsNullOrWhiteSpace(request.ThumbnailUrl) ? null : request.ThumbnailUrl.Trim();

        await _context.SaveChangesAsync(cancellationToken);

        var postCount = await _context.Posts.CountAsync(p => p.CategoryId == id, cancellationToken);

        return new AdminCategoryDetailDto
        {
            Id = category.Id,
            Name = category.Name,
            Slug = category.Slug,
            Description = category.Description,
            ThumbnailUrl = category.ThumbnailUrl,
            PostCount = postCount,
            CreatedAt = category.CreatedAt
        };
    }

    public async Task<(bool Success, string? ErrorMessage)> DeleteCategoryAsync(int id, CancellationToken cancellationToken = default)
    {
        var category = await _context.Categories
            .Include(c => c.Posts)
            .FirstOrDefaultAsync(c => c.Id == id, cancellationToken);

        if (category is null)
        {
            return (false, "Không tìm thấy chủ đề & danh mục yêu cầu.");
        }

        var postCount = category.Posts.Count;
        if (postCount > 0)
        {
            return (false, $"Không thể xóa danh mục \"{category.Name}\" vì đang có {postCount} bài viết thuộc danh mục này. Vui lòng chuyển bài viết sang danh mục khác trước khi xóa.");
        }

        _context.Categories.Remove(category);
        await _context.SaveChangesAsync(cancellationToken);

        return (true, null);
    }

    private async Task<string> GenerateUniqueSlugAsync(string baseSlug, int? excludeId, CancellationToken cancellationToken)
    {
        var slug = baseSlug;
        var counter = 1;

        while (true)
        {
            var query = _context.Categories.Where(c => c.Slug == slug);
            if (excludeId.HasValue)
            {
                query = query.Where(c => c.Id != excludeId.Value);
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
