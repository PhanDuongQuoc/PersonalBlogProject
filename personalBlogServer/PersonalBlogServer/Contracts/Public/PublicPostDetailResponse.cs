using System;
using System.Collections.Generic;

namespace PersonalBlogServer.Contracts.Public;

public sealed record PublicPostDetailResponse(
    int Id,
    string Title,
    string Slug,
    string Content,
    string? Excerpt,
    string? ThumbnailUrl,
    DateTime? PublishedAt,
    int ViewCount,
    PublicCategoryDetailDto Category,
    PublicAuthorDetailDto Author,
    IReadOnlyList<PublicTagDetailDto> Tags,
    IReadOnlyList<PublicCommentDetailDto> Comments,
    IReadOnlyList<PublicPostSummaryDto> RelatedPosts);

public sealed record PublicCategoryDetailDto(int Id, string Name, string Slug);

public sealed record PublicAuthorDetailDto(
    int Id,
    string Username,
    string Name,
    string Role,
    string? AvatarUrl,
    string? Bio,
    string? JobTitle);

public sealed record PublicTagDetailDto(int Id, string Name, string Slug);

public sealed record PublicCommentDetailDto(
    int Id,
    string? GuestName,
    string Content,
    DateTime CreatedAt);

public sealed record PublicPostSummaryDto(
    int Id,
    string Title,
    string Slug,
    string? Excerpt,
    string? ThumbnailUrl,
    string Category,
    DateTime? PublishedAt,
    int ViewCount);

public sealed record CreateCommentRequest(
    string Name,
    string Email,
    string Content);
