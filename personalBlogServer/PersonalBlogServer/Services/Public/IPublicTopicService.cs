using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using PersonalBlogServer.Contracts.Public;

namespace PersonalBlogServer.Services.Public;

public interface IPublicTopicService
{
    /// <summary>
    /// Lấy danh sách tất cả các chủ đề kèm số lượng bài viết, tổng views và bài viết gần nhất.
    /// </summary>
    /// <param name="username">Username tác giả (tùy chọn)</param>
    /// <param name="search">Từ khóa tìm kiếm chủ đề (tùy chọn)</param>
    /// <param name="cancellationToken">Cancellation token</param>
    Task<IReadOnlyList<PublicTopicSummaryResponse>> GetTopicsAsync(
        string? username = null,
        string? search = null,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Lấy chi tiết một chủ đề theo slug kèm danh sách bài viết (phân trang, lọc, tìm kiếm).
    /// </summary>
    /// <param name="slug">Slug của chủ đề (ví dụ: "vue-js", "dotnet-core")</param>
    /// <param name="username">Username tác giả (tùy chọn)</param>
    /// <param name="page">Số trang hiện tại (bắt đầu từ 1)</param>
    /// <param name="pageSize">Số lượng bài viết trên 1 trang</param>
    /// <param name="sortBy">Kiểu sắp xếp: "latest" (mới nhất), "popular" (nhiều view nhất), "oldest" (cũ nhất)</param>
    /// <param name="search">Từ khóa tìm kiếm bài viết trong chủ đề</param>
    /// <param name="tag">Slug thẻ tag cần lọc (tùy chọn)</param>
    /// <param name="cancellationToken">Cancellation token</param>
    Task<PublicTopicDetailResponse?> GetTopicBySlugAsync(
        string slug,
        string? username = null,
        int page = 1,
        int pageSize = 6,
        string? sortBy = "latest",
        string? search = null,
        string? tag = null,
        CancellationToken cancellationToken = default);
}
