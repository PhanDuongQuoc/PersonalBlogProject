using System.Threading;
using System.Threading.Tasks;
using PersonalBlogServer.Contracts.Admin;

namespace PersonalBlogServer.Services.Admin;

public interface IAdminTagService
{
    Task<PagedResult<AdminTagListDto>> GetTagsAsync(string? search, int page, int pageSize, CancellationToken cancellationToken = default);
    Task<AdminTagStatsDto> GetStatsAsync(CancellationToken cancellationToken = default);
    Task<AdminTagDetailDto?> GetTagByIdAsync(int id, CancellationToken cancellationToken = default);
    Task<AdminTagDetailDto> CreateTagAsync(CreateTagRequest request, CancellationToken cancellationToken = default);
    Task<AdminTagDetailDto?> UpdateTagAsync(int id, UpdateTagRequest request, CancellationToken cancellationToken = default);
    Task<bool> DeleteTagAsync(int id, CancellationToken cancellationToken = default);
}
