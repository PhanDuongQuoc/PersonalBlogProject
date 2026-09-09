using System;
using System.Collections.Generic;

namespace PersonalBlogServer.Contracts.Public;

/// <summary>
/// DTO đại diện cho 1 chủ đề ở trang danh sách tất cả chủ đề (/topics)
/// </summary>
public sealed record PublicTopicSummaryResponse(
    int Id,
    string Name,
    string Slug,
    string? Description,
    int PostCount,
    int TotalViews,
    DateTime? LatestPublishedAt,
    IReadOnlyList<PublicPostSummaryDto> RecentPosts);

/// <summary>
/// DTO đại diện cho dữ liệu toàn trang chi tiết chủ đề (/topics/{slug})
/// </summary>
public sealed record PublicTopicDetailResponse(
    PublicTopicInfoDto Topic,
    PublicTopicPostsPagedDto Posts,
    IReadOnlyList<PublicTagDetailDto> AvailableTags,
    IReadOnlyList<PublicTopicNavDto> OtherTopics);

/// <summary>
/// Thông tin cơ bản và số liệu của chủ đề hiện tại
/// </summary>
public sealed record PublicTopicInfoDto(
    int Id,
    string Name,
    string Slug,
    string? Description,
    int TotalPosts,
    int TotalViews);

/// <summary>
/// Từng bài viết thuộc chủ đề kèm danh sách tags
/// </summary>
public sealed record PublicTopicPostItemDto(
    int Id,
    string Title,
    string Slug,
    string? Excerpt,
    string? ThumbnailUrl,
    DateTime? PublishedAt,
    int ViewCount,
    IReadOnlyList<PublicTagDetailDto> Tags);

/// <summary>
/// Danh sách bài viết có phân trang
/// </summary>
public sealed record PublicTopicPostsPagedDto(
    IReadOnlyList<PublicTopicPostItemDto> Items,
    int CurrentPage,
    int PageSize,
    int TotalItems,
    int TotalPages);

/// <summary>
/// Thông tin rút gọn của chủ đề khác để điều hướng
/// </summary>
public sealed record PublicTopicNavDto(
    int Id,
    string Name,
    string Slug,
    int PostCount);
