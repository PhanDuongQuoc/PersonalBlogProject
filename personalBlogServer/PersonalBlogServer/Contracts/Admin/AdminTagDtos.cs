using System;

namespace PersonalBlogServer.Contracts.Admin;

public class AdminTagListDto
{
    public int Id { get; set; }
    public string Name { get; set; } = null!;
    public string Slug { get; set; } = null!;
    public int PostCount { get; set; }
    public DateTime CreatedAt { get; set; }
}

public class AdminTagDetailDto
{
    public int Id { get; set; }
    public string Name { get; set; } = null!;
    public string Slug { get; set; } = null!;
    public int PostCount { get; set; }
    public DateTime CreatedAt { get; set; }
}

public class CreateTagRequest
{
    public string Name { get; set; } = null!;
    public string? Slug { get; set; }
}

public class UpdateTagRequest
{
    public string Name { get; set; } = null!;
    public string? Slug { get; set; }
}

public class AdminTagStatsDto
{
    public int TotalTags { get; set; }
    public int TotalAssignedPosts { get; set; }
    public int TagsWithNoPosts { get; set; }
    public string? TopTagName { get; set; }
    public int TopTagPostCount { get; set; }
}
