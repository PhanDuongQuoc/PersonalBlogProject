using Microsoft.EntityFrameworkCore;
using PersonalBlogServer.Contracts.Public;
using PersonalBlogServer.Models;

namespace PersonalBlogServer.Services.Public;

public sealed class PublicAboutService : IPublicAboutService
{
    private readonly AppDbContext _context;

    public PublicAboutService(AppDbContext context)
    {
        _context = context;
    }

    public async Task<PublicAboutResponse?> GetAboutAsync(string? username, CancellationToken cancellationToken = default)
    {
        var usersQuery = _context.Users
            .AsNoTracking()
            .Where(user => user.IsActive && user.Role == "Author");

        if (!string.IsNullOrWhiteSpace(username))
        {
            usersQuery = usersQuery.Where(user => user.Username == username);
        }

        var owner = await usersQuery
            .OrderBy(user => user.CreatedAt)
            .FirstOrDefaultAsync(cancellationToken);

        if (owner is null)
        {
            return null;
        }

        var skills = await _context.UserSkills
            .AsNoTracking()
            .Where(s => s.UserId == owner.Id)
            .OrderBy(s => s.DisplayOrder)
            .ThenBy(s => s.Category)
            .Select(s => new PublicAboutSkillResponse(
                s.Id,
                s.Name,
                s.Category,
                s.Proficiency,
                s.Icon,
                s.DisplayOrder))
            .ToListAsync(cancellationToken);

        var experiences = await _context.UserExperiences
            .AsNoTracking()
            .Where(e => e.UserId == owner.Id)
            .OrderBy(e => e.DisplayOrder)
            .ThenByDescending(e => e.IsCurrent)
            .Select(e => new PublicAboutExperienceResponse(
                e.Id,
                e.Role,
                e.Company,
                e.Location,
                e.StartDate,
                e.EndDate,
                e.IsCurrent,
                e.Description,
                e.Technologies,
                e.DisplayOrder))
            .ToListAsync(cancellationToken);

        var educations = await _context.UserEducations
            .AsNoTracking()
            .Where(edu => edu.UserId == owner.Id)
            .OrderBy(edu => edu.DisplayOrder)
            .Select(edu => new PublicAboutEducationResponse(
                edu.Id,
                edu.Institution,
                edu.Degree,
                edu.StartYear,
                edu.EndYear,
                edu.Description,
                edu.DisplayOrder))
            .ToListAsync(cancellationToken);

        var publishedPostCount = await _context.Posts
            .AsNoTracking()
            .CountAsync(post => post.AuthorId == owner.Id && post.Status == "Published", cancellationToken);

        var categoryCount = await _context.Posts
            .AsNoTracking()
            .Where(post => post.AuthorId == owner.Id && post.Status == "Published")
            .Select(post => post.CategoryId)
            .Distinct()
            .CountAsync(cancellationToken);

        var totalViewCount = await _context.Posts
            .AsNoTracking()
            .Where(post => post.AuthorId == owner.Id && post.Status == "Published")
            .Select(post => (int?)post.ViewCount)
            .SumAsync(cancellationToken) ?? 0;

        var profile = new PublicAboutProfileResponse(
            owner.Id,
            owner.Username,
            owner.DisplayName ?? owner.Username,
            owner.Role,
            owner.JobTitle ?? "Fullstack Web Developer",
            owner.Bio,
            owner.AboutStory,
            owner.Location ?? "Việt Nam",
            owner.Phone,
            owner.Email,
            owner.AvatarUrl,
            owner.CvUrl,
            owner.GithubUrl,
            owner.LinkedinUrl,
            owner.FacebookUrl,
            owner.TwitterUrl,
            owner.WebsiteUrl,
            owner.YearsOfExperience ?? 0);

        var stats = new PublicAboutStatsResponse(
            publishedPostCount,
            categoryCount,
            totalViewCount,
            owner.YearsOfExperience ?? 0);

        return new PublicAboutResponse(profile, skills, experiences, educations, stats);
    }
}
