using System;
using System.Collections.Generic;

namespace PersonalBlogServer.Contracts.Admin;

public class AdminAnalyticsSummaryDto
{
    public long TotalViews { get; set; }
    public double ViewsGrowthRate { get; set; }
    public int TotalPosts { get; set; }
    public int PublishedPosts { get; set; }
    public int DraftPosts { get; set; }
    public int ArchivedPosts { get; set; }
    public int TotalComments { get; set; }
    public int ApprovedComments { get; set; }
    public int PendingComments { get; set; }
    public int TotalCategories { get; set; }
    public int TotalTags { get; set; }
}

public class ViewsTrendPointDto
{
    public string DateLabel { get; set; } = string.Empty;
    public long Views { get; set; }
    public long UniqueReaders { get; set; }
}

public class ViewsTrendResponse
{
    public string Period { get; set; } = "30d";
    public long TotalViewsInPeriod { get; set; }
    public List<ViewsTrendPointDto> DataPoints { get; set; } = new();
}

public class CategoryAnalyticsDto
{
    public int CategoryId { get; set; }
    public string CategoryName { get; set; } = string.Empty;
    public string Slug { get; set; } = string.Empty;
    public int PostCount { get; set; }
    public long TotalViews { get; set; }
    public double Percentage { get; set; }
}

public class MonthlyCommentsDto
{
    public int Month { get; set; }
    public string MonthLabel { get; set; } = string.Empty;
    public int TotalComments { get; set; }
    public int ApprovedComments { get; set; }
}

public class TopPerformingPostDto
{
    public int Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Slug { get; set; } = string.Empty;
    public string CategoryName { get; set; } = string.Empty;
    public long ViewCount { get; set; }
    public int CommentCount { get; set; }
    public DateTime? PublishedAt { get; set; }
    public string? ThumbnailUrl { get; set; }
}
