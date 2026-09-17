using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PersonalBlogServer.Services.Admin;

namespace PersonalBlogServer.Controllers;

[ApiController]
[Route("api/admin/analytics")]
public class AdminAnalyticsController : ControllerBase
{
    private readonly IAdminAnalyticsService _analyticsService;

    public AdminAnalyticsController(IAdminAnalyticsService analyticsService)
    {
        _analyticsService = analyticsService;
    }

    [HttpGet("summary")]
    public async Task<IActionResult> GetSummary()
    {
        var summary = await _analyticsService.GetSummaryAsync();
        return Ok(summary);
    }

    [HttpGet("views-trend")]
    public async Task<IActionResult> GetViewsTrend([FromQuery] string? period)
    {
        var trend = await _analyticsService.GetViewsTrendAsync(period ?? "monthly");
        return Ok(trend);
    }

    [HttpGet("category-distribution")]
    public async Task<IActionResult> GetCategoryDistribution()
    {
        var distribution = await _analyticsService.GetCategoryDistributionAsync();
        return Ok(distribution);
    }

    [HttpGet("comments-trend")]
    public async Task<IActionResult> GetCommentsTrend([FromQuery] string? period, [FromQuery] int? year)
    {
        var trend = await _analyticsService.GetMonthlyCommentsAsync(period ?? "monthly", year);
        return Ok(trend);
    }

    [HttpGet("contacts-trend")]
    public async Task<IActionResult> GetContactsTrend([FromQuery] string? period, [FromQuery] int? year)
    {
        var trend = await _analyticsService.GetMonthlyContactsAsync(period ?? "monthly", year);
        return Ok(trend);
    }

    [HttpGet("top-posts")]
    public async Task<IActionResult> GetTopPosts([FromQuery] int limit = 5)
    {
        var posts = await _analyticsService.GetTopPostsAsync(limit);
        return Ok(posts);
    }
}
