using System.Threading;
using System.Threading.Tasks;
using PersonalBlogServer.Contracts.Admin;

namespace PersonalBlogServer.Services.Admin;

public interface IAdminCommentService
{
    Task<PagedResult<AdminCommentListDto>> GetCommentsAsync(string? search, string? status, int? postId, int page, int pageSize, CancellationToken cancellationToken = default);
    Task<AdminCommentStatsDto> GetStatsAsync(CancellationToken cancellationToken = default);
    Task<AdminCommentDetailDto?> GetCommentByIdAsync(int id, CancellationToken cancellationToken = default);
    Task<bool> UpdateStatusAsync(int id, string status, CancellationToken cancellationToken = default);
    Task<bool> DeleteCommentAsync(int id, CancellationToken cancellationToken = default);
}
