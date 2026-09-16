using System.Threading;
using System.Threading.Tasks;
using PersonalBlogServer.Contracts.Admin;

namespace PersonalBlogServer.Services.Admin;

public interface ISiteSettingsService
{
    Task<SiteSettingsDto> GetSettingsAsync(CancellationToken cancellationToken = default);
    Task<SiteSettingsDto> UpdateSettingsAsync(SiteSettingsDto newSettings, CancellationToken cancellationToken = default);
    Task<bool> ClearCacheAsync(CancellationToken cancellationToken = default);
}
