using System.Collections.Generic;
using System.Threading.Tasks;
using PersonalBlogServer.Contracts.Admin;

namespace PersonalBlogServer.Services.Admin;

public interface IAdminAnalyticsService
{
    Task<AdminAnalyticsSummaryDto> GetSummaryAsync();
    Task<ViewsTrendResponse> GetViewsTrendAsync(string period);
    Task<List<CategoryAnalyticsDto>> GetCategoryDistributionAsync();
    Task<List<MonthlyCommentsDto>> GetMonthlyCommentsAsync(int? year);
    Task<List<TopPerformingPostDto>> GetTopPostsAsync(int limit = 5);
}
