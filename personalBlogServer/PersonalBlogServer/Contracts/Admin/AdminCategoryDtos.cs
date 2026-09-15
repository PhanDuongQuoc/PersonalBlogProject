using System;

namespace PersonalBlogServer.Contracts.Admin;

public class AdminCategoryListDto
{
    public int Id { get; set; }
    public string Name { get; set; } = null!;
    public string Slug { get; set; } = null!;
    public string? Description { get; set; }
    public string? ThumbnailUrl { get; set; }
    public int PostCount { get; set; }
    public DateTime CreatedAt { get; set; }
}

public class AdminCategoryDetailDto
{
    public int Id { get; set; }
    public string Name { get; set; } = null!;
    public string Slug { get; set; } = null!;
    public string? Description { get; set; }
    public string? ThumbnailUrl { get; set; }
    public int PostCount { get; set; }
    public DateTime CreatedAt { get; set; }
}

public class CreateCategoryRequest
{
    public string Name { get; set; } = null!;
    public string? Slug { get; set; }
    public string? Description { get; set; }
    public string? ThumbnailUrl { get; set; }
}

public class UpdateCategoryRequest
{
    public string Name { get; set; } = null!;
    public string? Slug { get; set; }
    public string? Description { get; set; }
    public string? ThumbnailUrl { get; set; }
}

public class AdminCategoryStatsDto
{
    public int TotalCategories { get; set; }
    public int TotalAssignedPosts { get; set; }
    public int CategoriesWithNoPosts { get; set; }
    public string? TopCategoryName { get; set; }
    public int TopCategoryPostCount { get; set; }
}
