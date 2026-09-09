using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using PersonalBlogServer.Contracts.Public;
using PersonalBlogServer.Models;

namespace PersonalBlogServer.Services.Public;

public sealed class PublicTopicService : IPublicTopicService
{
    private readonly AppDbContext _context;

    public PublicTopicService(AppDbContext context)
    {
        _context = context;
    }

    public async Task<IReadOnlyList<PublicTopicSummaryResponse>> GetTopicsAsync(
        string? username = null,
        string? search = null,
        CancellationToken cancellationToken = default)
    {
        var categoriesQuery = _context.Categories.AsNoTracking();

        if (!string.IsNullOrWhiteSpace(search))
        {
            var searchLower = search.Trim().ToLower();
            categoriesQuery = categoriesQuery.Where(c =>
                c.Name.ToLower().Contains(searchLower) ||
                (c.Description != null && c.Description.ToLower().Contains(searchLower)));
        }

        var categories = await categoriesQuery
            .Select(c => new
            {
                c.Id,
                c.Name,
                c.Slug,
                c.Description
            })
            .ToListAsync(cancellationToken);

        if (categories.Count == 0)
        {
            return Array.Empty<PublicTopicSummaryResponse>();
        }

        var categoryIds = categories.Select(c => c.Id).ToList();

        // Query published posts for these categories in 1 efficient query
        var postsQuery = _context.Posts
            .AsNoTracking()
            .Where(p => categoryIds.Contains(p.CategoryId) && p.Status == "Published");

        if (!string.IsNullOrWhiteSpace(username))
        {
            postsQuery = postsQuery.Where(p => p.Author.Username == username);
        }

        var allPosts = await postsQuery
            .Select(p => new
            {
                p.Id,
                p.CategoryId,
                p.Title,
                p.Slug,
                p.Excerpt,
                p.ThumbnailUrl,
                p.PublishedAt,
                p.ViewCount
            })
            .ToListAsync(cancellationToken);

        var postsByCategory = allPosts
            .GroupBy(p => p.CategoryId)
            .ToDictionary(g => g.Key, g => g.OrderByDescending(p => p.PublishedAt).ToList());

        var result = categories
            .Select(c =>
            {
                var categoryPosts = postsByCategory.TryGetValue(c.Id, out var pList) ? pList : new();
                var postCount = categoryPosts.Count;
                var totalViews = categoryPosts.Sum(p => p.ViewCount);
                var latestPublishedAt = categoryPosts.FirstOrDefault()?.PublishedAt;
                var recentPosts = categoryPosts
                    .Take(3)
                    .Select(p => new PublicPostSummaryDto(
                        p.Id,
                        p.Title,
                        p.Slug,
                        p.Excerpt,
                        p.ThumbnailUrl,
                        c.Name,
                        p.PublishedAt,
                        p.ViewCount
                    ))
                    .ToList();

                return new PublicTopicSummaryResponse(
                    c.Id,
                    c.Name,
                    c.Slug,
                    c.Description,
                    postCount,
                    totalViews,
                    latestPublishedAt,
                    recentPosts
                );
            })
            .OrderByDescending(t => t.PostCount)
            .ThenBy(t => t.Name)
            .ToList();

        return result;
    }

    public async Task<PublicTopicDetailResponse?> GetTopicBySlugAsync(
        string slug,
        string? username = null,
        int page = 1,
        int pageSize = 6,
        string? sortBy = "latest",
        string? search = null,
        string? tag = null,
        CancellationToken cancellationToken = default)
    {
        if (page < 1) page = 1;
        if (pageSize < 1) pageSize = 6;
        if (pageSize > 50) pageSize = 50;

        // 1. Tìm Category theo Slug
        var category = await _context.Categories
            .AsNoTracking()
            .FirstOrDefaultAsync(c => c.Slug == slug, cancellationToken);

        if (category is null)
        {
            return null;
        }

        // 2. Base query cho Published posts
        var basePostsQuery = _context.Posts
            .AsNoTracking()
            .Where(p => p.CategoryId == category.Id && p.Status == "Published");

        if (!string.IsNullOrWhiteSpace(username))
        {
            basePostsQuery = basePostsQuery.Where(p => p.Author.Username == username);
        }

        var totalTopicPosts = await basePostsQuery.CountAsync(cancellationToken);
        var totalTopicViews = await basePostsQuery.Select(p => (int?)p.ViewCount).SumAsync(cancellationToken) ?? 0;

        // 3. Filter query
        var filteredQuery = basePostsQuery;

        if (!string.IsNullOrWhiteSpace(search))
        {
            var searchLower = search.Trim().ToLower();
            filteredQuery = filteredQuery.Where(p =>
                p.Title.ToLower().Contains(searchLower) ||
                (p.Excerpt != null && p.Excerpt.ToLower().Contains(searchLower)));
        }

        if (!string.IsNullOrWhiteSpace(tag))
        {
            filteredQuery = filteredQuery.Where(p => p.Tags.Any(t => t.Slug == tag));
        }

        filteredQuery = (sortBy?.ToLower()) switch
        {
            "popular" => filteredQuery.OrderByDescending(p => p.ViewCount).ThenByDescending(p => p.PublishedAt),
            "oldest" => filteredQuery.OrderBy(p => p.PublishedAt).ThenByDescending(p => p.Id),
            _ => filteredQuery.OrderByDescending(p => p.PublishedAt).ThenByDescending(p => p.Id)
        };

        var totalItems = await filteredQuery.CountAsync(cancellationToken);
        var totalPages = (int)Math.Ceiling(totalItems / (double)pageSize);

        // 4. Lấy danh sách bài viết phân trang
        var postEntities = await filteredQuery
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .Select(p => new
            {
                p.Id,
                p.Title,
                p.Slug,
                p.Excerpt,
                p.ThumbnailUrl,
                p.PublishedAt,
                p.ViewCount,
                Tags = p.Tags.Select(t => new { t.Id, t.Name, t.Slug }).ToList()
            })
            .ToListAsync(cancellationToken);

        var postItems = postEntities
            .Select(p => new PublicTopicPostItemDto(
                p.Id,
                p.Title,
                p.Slug,
                p.Excerpt,
                p.ThumbnailUrl,
                p.PublishedAt,
                p.ViewCount,
                p.Tags.Select(t => new PublicTagDetailDto(t.Id, t.Name, t.Slug)).ToList()
            ))
            .ToList();

        // 5. Lấy danh sách tags liên quan qua Tag query (hoàn toàn an toàn cho PostgreSQL)
        var availableTags = await _context.Tags
            .AsNoTracking()
            .Where(t => t.Posts.Any(p => p.CategoryId == category.Id && p.Status == "Published" && (string.IsNullOrWhiteSpace(username) || p.Author.Username == username)))
            .Select(t => new PublicTagDetailDto(t.Id, t.Name, t.Slug))
            .Distinct()
            .ToListAsync(cancellationToken);

        // 6. Lấy các chủ đề khác
        var otherCategories = await _context.Categories
            .AsNoTracking()
            .Where(c => c.Id != category.Id)
            .Select(c => new
            {
                c.Id,
                c.Name,
                c.Slug,
                PostCount = c.Posts.Count(p => p.Status == "Published" && (string.IsNullOrWhiteSpace(username) || p.Author.Username == username))
            })
            .OrderByDescending(c => c.PostCount)
            .Take(5)
            .ToListAsync(cancellationToken);

        var otherTopics = otherCategories
            .Select(c => new PublicTopicNavDto(c.Id, c.Name, c.Slug, c.PostCount))
            .ToList();

        var topicInfo = new PublicTopicInfoDto(
            category.Id,
            category.Name,
            category.Slug,
            category.Description,
            totalTopicPosts,
            totalTopicViews
        );

        var pagedResult = new PublicTopicPostsPagedDto(
            postItems,
            page,
            pageSize,
            totalItems,
            totalPages
        );

        return new PublicTopicDetailResponse(
            topicInfo,
            pagedResult,
            availableTags,
            otherTopics
        );
    }
}
