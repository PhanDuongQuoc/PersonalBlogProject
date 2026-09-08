using Microsoft.EntityFrameworkCore;
using PersonalBlogServer.Contracts.Public;
using PersonalBlogServer.Models;

namespace PersonalBlogServer.Services.Public;

public sealed class PublicLandingService : IPublicLandingService
{
    private readonly AppDbContext _context;

    public PublicLandingService(AppDbContext context)
    {
        _context = context;
    }

    public async Task<PublicLandingResponse?> GetLandingAsync(string? username, CancellationToken cancellationToken = default)
    {
        var users = _context.Users
            .AsNoTracking()
            .Where(user => user.IsActive && user.Role == "Author");

        if (!string.IsNullOrWhiteSpace(username))
        {
            users = users.Where(user => user.Username == username);
        }

        var owner = await users
            .OrderBy(user => user.CreatedAt)
            .FirstOrDefaultAsync(cancellationToken);

        if (owner is null)
        {
            return null;
        }

        var featuredPosts = await _context.Posts
            .AsNoTracking()
            .Where(post => post.AuthorId == owner.Id && post.Status == "Published")
            .OrderByDescending(post => post.PublishedAt)
            .Take(3)
            .Select(post => new PublicPostResponse(
                post.Id,
                post.Title,
                post.Slug,
                post.Excerpt,
                post.ThumbnailUrl,
                post.Category.Name,
                post.PublishedAt,
                post.ViewCount))
            .ToListAsync(cancellationToken);

        var categories = await _context.Posts
            .AsNoTracking()
            .Where(post => post.AuthorId == owner.Id && post.Status == "Published")
            .GroupBy(post => new { post.Category.Name, post.Category.Slug })
            .OrderByDescending(group => group.Count())
            .Select(group => new PublicCategoryResponse(group.Key.Name, group.Key.Slug, group.Count()))
            .ToListAsync(cancellationToken);

        var publishedPostCount = await _context.Posts
            .AsNoTracking()
            .CountAsync(post => post.AuthorId == owner.Id && post.Status == "Published", cancellationToken);

        var totalViewCount = await _context.Posts
            .AsNoTracking()
            .Where(post => post.AuthorId == owner.Id && post.Status == "Published")
            .Select(post => (int?)post.ViewCount)
            .SumAsync(cancellationToken) ?? 0;

        var profile = new PublicProfileResponse(
            owner.Id,
            owner.Username,
            owner.DisplayName ?? owner.Username,
            owner.Role,
            owner.JobTitle ?? "Fullstack Web Developer",
            owner.Bio,
            owner.Location,
            owner.Phone,
            owner.AvatarUrl,
            owner.Email,
            owner.CvUrl,
            owner.GithubUrl,
            owner.LinkedinUrl,
            owner.FacebookUrl,
            owner.TwitterUrl,
            owner.WebsiteUrl,
            owner.YearsOfExperience ?? 0);

        return new PublicLandingResponse(
            profile,
            featuredPosts,
            categories,
            new PublicLandingStatsResponse(
                publishedPostCount,
                categories.Count,
                totalViewCount,
                owner.YearsOfExperience ?? 0));
    }
}
