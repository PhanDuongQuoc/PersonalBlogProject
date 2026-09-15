using System;
using System.Collections.Generic;

namespace PersonalBlogServer.Contracts.Admin;

public sealed record AdminPostSummaryDto(
    int Id,
    string Title,
    string Slug,
    string? Excerpt,
    string? ThumbnailUrl,
    string Status,
    int CategoryId,
    string CategoryName,
    int AuthorId,
    string AuthorName,
    int ViewCount,
    DateTime? PublishedAt,
    DateTime CreatedAt,
    DateTime? UpdatedAt,
    List<string> Tags
);

public sealed record AdminTagSummaryDto(
    int Id,
    string Name,
    string Slug
);

public sealed record AdminCategorySummaryDto(
    int Id,
    string Name,
    string Slug
);

public sealed record AdminPostDetailDto(
    int Id,
    string Title,
    string Slug,
    string Content,
    string? Excerpt,
    string? ThumbnailUrl,
    string Status,
    AdminCategorySummaryDto Category,
    int AuthorId,
    string AuthorName,
    int ViewCount,
    DateTime? PublishedAt,
    DateTime CreatedAt,
    DateTime? UpdatedAt,
    List<AdminTagSummaryDto> Tags
);

public sealed record CreatePostRequest(
    string Title,
    string? Slug,
    string Content,
    string? Excerpt,
    string? ThumbnailUrl,
    string? Status,
    int CategoryId,
    List<string>? TagNames
);

public sealed record UpdatePostRequest(
    string Title,
    string? Slug,
    string Content,
    string? Excerpt,
    string? ThumbnailUrl,
    string? Status,
    int CategoryId,
    List<string>? TagNames
);

public sealed record UpdatePostStatusRequest(
    string Status
);

public sealed record AdminPostStatsDto(
    int TotalPosts,
    int PublishedPosts,
    int DraftPosts,
    long TotalViews
);

public sealed record PagedResult<T>(
    List<T> Items,
    int TotalCount,
    int Page,
    int PageSize,
    int TotalPages
);
