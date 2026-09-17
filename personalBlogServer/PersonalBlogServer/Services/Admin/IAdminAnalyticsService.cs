using System.Collections.Generic;
using System.Threading.Tasks;
using PersonalBlogServer.Contracts.Admin;

namespace PersonalBlogServer.Services.Admin;

public interface IAdminAnalyticsService
{
    Task<AdminAnalyticsSummaryDto> GetSummaryAsync();
    Task<ViewsTrendResponse> GetViewsTrendAsync(string period);
    Task<List<CategoryAnalyticsDto>> GetCategoryDistributionAsync();
    Task<List<MonthlyCommentsDto>> GetMonthlyCommentsAsync(string? period = "monthly", int? year = null);
    Task<List<MonthlyContactsDto>> GetMonthlyContactsAsync(string? period = "monthly", int? year = null);
    Task<List<TopPerformingPostDto>> GetTopPostsAsync(int limit = 5);
}
