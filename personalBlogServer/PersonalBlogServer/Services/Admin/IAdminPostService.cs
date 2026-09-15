using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using PersonalBlogServer.Contracts.Admin;

namespace PersonalBlogServer.Services.Admin;

public interface IAdminPostService
{
    Task<PagedResult<AdminPostSummaryDto>> GetPostsAsync(
        string? search,
        int? categoryId,
        string? status,
        int page,
        int pageSize,
        CancellationToken cancellationToken = default);

    Task<AdminPostDetailDto?> GetPostByIdAsync(int id, CancellationToken cancellationToken = default);

    Task<AdminPostDetailDto> CreatePostAsync(int authorId, CreatePostRequest request, CancellationToken cancellationToken = default);

    Task<AdminPostDetailDto?> UpdatePostAsync(int id, UpdatePostRequest request, CancellationToken cancellationToken = default);

    Task<bool> UpdateStatusAsync(int id, string status, CancellationToken cancellationToken = default);

    Task<bool> DeletePostAsync(int id, CancellationToken cancellationToken = default);

    Task<AdminPostStatsDto> GetStatsAsync(CancellationToken cancellationToken = default);

    Task<List<AdminCategorySummaryDto>> GetCategoriesAsync(CancellationToken cancellationToken = default);

    Task<List<AdminTagSummaryDto>> GetTagsAsync(CancellationToken cancellationToken = default);
}
