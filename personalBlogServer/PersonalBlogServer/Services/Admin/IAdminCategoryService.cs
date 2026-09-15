using System.Threading;
using System.Threading.Tasks;
using PersonalBlogServer.Contracts.Admin;

namespace PersonalBlogServer.Services.Admin;

public interface IAdminCategoryService
{
    Task<PagedResult<AdminCategoryListDto>> GetCategoriesAsync(string? search, int page, int pageSize, CancellationToken cancellationToken = default);
    Task<AdminCategoryStatsDto> GetStatsAsync(CancellationToken cancellationToken = default);
    Task<AdminCategoryDetailDto?> GetCategoryByIdAsync(int id, CancellationToken cancellationToken = default);
    Task<AdminCategoryDetailDto> CreateCategoryAsync(CreateCategoryRequest request, CancellationToken cancellationToken = default);
    Task<AdminCategoryDetailDto?> UpdateCategoryAsync(int id, UpdateCategoryRequest request, CancellationToken cancellationToken = default);
    Task<(bool Success, string? ErrorMessage)> DeleteCategoryAsync(int id, CancellationToken cancellationToken = default);
}
