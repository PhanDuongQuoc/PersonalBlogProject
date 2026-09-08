namespace PersonalBlogServer.Contracts.Public;

public sealed record PublicLandingResponse(
    PublicProfileResponse Profile,
    IReadOnlyList<PublicPostResponse> FeaturedPosts,
    IReadOnlyList<PublicCategoryResponse> Categories,
    PublicLandingStatsResponse Stats);

public sealed record PublicProfileResponse(
    int Id,
    string Username,
    string Name,
    string Role,
    string? JobTitle,
    string? Bio,
    string? Location,
    string? Phone,
    string? AvatarUrl,
    string Email,
    string? CvUrl,
    string? GithubUrl,
    string? LinkedinUrl,
    string? FacebookUrl,
    string? TwitterUrl,
    string? WebsiteUrl,
    int YearsOfExperience);

public sealed record PublicPostResponse(
    int Id,
    string Title,
    string Slug,
    string? Excerpt,
    string? ThumbnailUrl,
    string Category,
    DateTime? PublishedAt,
    int ViewCount);

public sealed record PublicCategoryResponse(string Name, string Slug, int PostCount);

public sealed record PublicLandingStatsResponse(
    int PublishedPostCount,
    int CategoryCount,
    int TotalViewCount,
    int YearsOfExperience);
