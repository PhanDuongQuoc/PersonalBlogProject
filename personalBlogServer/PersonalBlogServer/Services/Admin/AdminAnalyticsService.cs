using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using PersonalBlogServer.Contracts.Admin;
using PersonalBlogServer.Models;

namespace PersonalBlogServer.Services.Admin;

public class AdminAnalyticsService : IAdminAnalyticsService
{
    private readonly AppDbContext _context;

    public AdminAnalyticsService(AppDbContext context)
    {
        _context = context;
    }

    public async Task<AdminAnalyticsSummaryDto> GetSummaryAsync()
    {
        var totalViews = await _context.Posts.SumAsync(p => (long)p.ViewCount);
        var totalPosts = await _context.Posts.CountAsync();
        var publishedPosts = await _context.Posts.CountAsync(p => p.Status == "Published");
        var draftPosts = await _context.Posts.CountAsync(p => p.Status == "Draft");
        var archivedPosts = await _context.Posts.CountAsync(p => p.Status == "Archived");

        var totalComments = await _context.Comments.CountAsync();
        var approvedComments = await _context.Comments.CountAsync(c => c.Status == "Approved");
        var pendingComments = await _context.Comments.CountAsync(c => c.Status == "Pending");

        var totalCategories = await _context.Categories.CountAsync();
        var totalTags = await _context.Tags.CountAsync();

        var totalContacts = await _context.ContactMessages.CountAsync();
        var unreadContacts = await _context.ContactMessages.CountAsync(c => c.Status == "Unread");
        var repliedContacts = await _context.ContactMessages.CountAsync(c => c.Status == "Replied");

        return new AdminAnalyticsSummaryDto
        {
            TotalViews = totalViews,
            ViewsGrowthRate = 0.0,
            TotalPosts = totalPosts,
            PublishedPosts = publishedPosts,
            DraftPosts = draftPosts,
            ArchivedPosts = archivedPosts,
            TotalComments = totalComments,
            ApprovedComments = approvedComments,
            PendingComments = pendingComments,
            TotalCategories = totalCategories,
            TotalTags = totalTags,
            TotalContacts = totalContacts,
            UnreadContacts = unreadContacts,
            RepliedContacts = repliedContacts
        };
    }

    public async Task<ViewsTrendResponse> GetViewsTrendAsync(string period)
    {
        var normalizedPeriod = (period ?? "monthly").ToLower().Trim();
        var posts = await _context.Posts.AsNoTracking().ToListAsync();
        var totalViews = posts.Sum(p => (long)p.ViewCount);

        var response = new ViewsTrendResponse
        {
            Period = normalizedPeriod,
            TotalViewsInPeriod = totalViews,
            DataPoints = new List<ViewsTrendPointDto>()
        };

        var now = DateTime.UtcNow;

        if (normalizedPeriod == "7d")
        {
            // 7 ngày gần nhất (từ 6 ngày trước đến hôm nay)
            for (int i = 6; i >= 0; i--)
            {
                var targetDate = now.AddDays(-i).Date;
                var label = targetDate.ToString("dd/MM");

                var dayPosts = posts.Where(p => (p.PublishedAt ?? p.CreatedAt).ToUniversalTime().Date == targetDate).ToList();
                var views = dayPosts.Sum(p => (long)p.ViewCount);

                response.DataPoints.Add(new ViewsTrendPointDto
                {
                    DateLabel = label,
                    Views = views,
                    UniqueReaders = views
                });
            }
        }
        else if (normalizedPeriod == "30d")
        {
            // 30 ngày qua (chia thành 6 mốc 5 ngày)
            for (int i = 5; i >= 0; i--)
            {
                var startDate = now.AddDays(-(i + 1) * 5).Date;
                var endDate = now.AddDays(-i * 5).Date;
                var label = endDate.ToString("dd/MM");

                var intervalPosts = posts.Where(p =>
                {
                    var postDate = (p.PublishedAt ?? p.CreatedAt).ToUniversalTime().Date;
                    return postDate > startDate && postDate <= endDate;
                }).ToList();

                var views = intervalPosts.Sum(p => (long)p.ViewCount);

                response.DataPoints.Add(new ViewsTrendPointDto
                {
                    DateLabel = label,
                    Views = views,
                    UniqueReaders = views
                });
            }
        }
        else if (normalizedPeriod == "yearly")
        {
            // 4 năm gần nhất
            int currentYear = now.Year;
            for (int y = currentYear - 3; y <= currentYear; y++)
            {
                var yearPosts = posts.Where(p => (p.PublishedAt ?? p.CreatedAt).ToUniversalTime().Year == y).ToList();
                var views = yearPosts.Sum(p => (long)p.ViewCount);

                response.DataPoints.Add(new ViewsTrendPointDto
                {
                    DateLabel = y.ToString(),
                    Views = views,
                    UniqueReaders = views
                });
            }
        }
        else
        {
            // Mặc định: 12 tháng của năm hiện tại
            int currentYear = now.Year;
            string[] monthNames = { "Thg 1", "Thg 2", "Thg 3", "Thg 4", "Thg 5", "Thg 6", "Thg 7", "Thg 8", "Thg 9", "Thg 10", "Thg 11", "Thg 12" };

            for (int m = 1; m <= 12; m++)
            {
                var monthPosts = posts.Where(p =>
                {
                    var postDate = (p.PublishedAt ?? p.CreatedAt).ToUniversalTime();
                    return postDate.Year == currentYear && postDate.Month == m;
                }).ToList();

                var views = monthPosts.Sum(p => (long)p.ViewCount);

                response.DataPoints.Add(new ViewsTrendPointDto
                {
                    DateLabel = monthNames[m - 1],
                    Views = views,
                    UniqueReaders = views
                });
            }
        }

        return response;
    }

    public async Task<List<CategoryAnalyticsDto>> GetCategoryDistributionAsync()
    {
        var categories = await _context.Categories
            .Include(c => c.Posts)
            .OrderBy(c => c.Name)
            .ToListAsync();

        long totalViewsAll = categories.Sum(c => c.Posts.Sum(p => (long)p.ViewCount));

        var result = categories.Select(c =>
        {
            long catViews = c.Posts.Sum(p => (long)p.ViewCount);
            double pct = totalViewsAll > 0 ? Math.Round((double)catViews / totalViewsAll * 100, 1) : 0;

            return new CategoryAnalyticsDto
            {
                CategoryId = c.Id,
                CategoryName = c.Name,
                Slug = c.Slug,
                PostCount = c.Posts.Count,
                TotalViews = catViews,
                Percentage = pct
            };
        }).OrderByDescending(c => c.TotalViews).ToList();

        return result;
    }

    public async Task<List<MonthlyCommentsDto>> GetMonthlyCommentsAsync(string? period = "monthly", int? year = null)
    {
        var normalizedPeriod = (period ?? "monthly").ToLower().Trim();
        var allComments = await _context.Comments.AsNoTracking().ToListAsync();
        var now = DateTime.UtcNow;
        var result = new List<MonthlyCommentsDto>();

        if (normalizedPeriod == "7d")
        {
            for (int i = 6; i >= 0; i--)
            {
                var targetDate = now.AddDays(-i).Date;
                var label = targetDate.ToString("dd/MM");
                var dayComments = allComments.Where(c => c.CreatedAt.ToUniversalTime().Date == targetDate).ToList();

                result.Add(new MonthlyCommentsDto
                {
                    Month = 7 - i,
                    MonthLabel = label,
                    TotalComments = dayComments.Count,
                    ApprovedComments = dayComments.Count(c => c.Status == "Approved")
                });
            }
        }
        else if (normalizedPeriod == "30d")
        {
            for (int i = 5; i >= 0; i--)
            {
                var startDate = now.AddDays(-(i + 1) * 5).Date;
                var endDate = now.AddDays(-i * 5).Date;
                var label = endDate.ToString("dd/MM");

                var intervalComments = allComments.Where(c =>
                {
                    var cDate = c.CreatedAt.ToUniversalTime().Date;
                    return cDate > startDate && cDate <= endDate;
                }).ToList();

                result.Add(new MonthlyCommentsDto
                {
                    Month = 6 - i,
                    MonthLabel = label,
                    TotalComments = intervalComments.Count,
                    ApprovedComments = intervalComments.Count(c => c.Status == "Approved")
                });
            }
        }
        else if (normalizedPeriod == "yearly")
        {
            int currentYear = now.Year;
            for (int y = currentYear - 3; y <= currentYear; y++)
            {
                var yearComments = allComments.Where(c => c.CreatedAt.ToUniversalTime().Year == y).ToList();

                result.Add(new MonthlyCommentsDto
                {
                    Month = y,
                    MonthLabel = y.ToString(),
                    TotalComments = yearComments.Count,
                    ApprovedComments = yearComments.Count(c => c.Status == "Approved")
                });
            }
        }
        else
        {
            int targetYear = year ?? (int.TryParse(normalizedPeriod, out int parsedY) ? parsedY : now.Year);
            string[] monthNames = { "Thg 1", "Thg 2", "Thg 3", "Thg 4", "Thg 5", "Thg 6", "Thg 7", "Thg 8", "Thg 9", "Thg 10", "Thg 11", "Thg 12" };

            for (int m = 1; m <= 12; m++)
            {
                var monthComments = allComments.Where(c =>
                {
                    var cDate = c.CreatedAt.ToUniversalTime();
                    return cDate.Year == targetYear && cDate.Month == m;
                }).ToList();

                result.Add(new MonthlyCommentsDto
                {
                    Month = m,
                    MonthLabel = monthNames[m - 1],
                    TotalComments = monthComments.Count,
                    ApprovedComments = monthComments.Count(c => c.Status == "Approved")
                });
            }
        }

        return result;
    }

    public async Task<List<MonthlyContactsDto>> GetMonthlyContactsAsync(string? period = "monthly", int? year = null)
    {
        var normalizedPeriod = (period ?? "monthly").ToLower().Trim();
        var allMessages = await _context.ContactMessages.AsNoTracking().ToListAsync();
        var now = DateTime.UtcNow;
        var result = new List<MonthlyContactsDto>();

        if (normalizedPeriod == "7d")
        {
            for (int i = 6; i >= 0; i--)
            {
                var targetDate = now.AddDays(-i).Date;
                var label = targetDate.ToString("dd/MM");
                var dayMessages = allMessages.Where(c => (c.CreatedAt ?? now).ToUniversalTime().Date == targetDate).ToList();

                result.Add(new MonthlyContactsDto
                {
                    Month = 7 - i,
                    MonthLabel = label,
                    TotalMessages = dayMessages.Count,
                    RepliedMessages = dayMessages.Count(c => c.Status == "Replied"),
                    UnreadMessages = dayMessages.Count(c => c.Status == "Unread")
                });
            }
        }
        else if (normalizedPeriod == "30d")
        {
            for (int i = 5; i >= 0; i--)
            {
                var startDate = now.AddDays(-(i + 1) * 5).Date;
                var endDate = now.AddDays(-i * 5).Date;
                var label = endDate.ToString("dd/MM");

                var intervalMessages = allMessages.Where(c =>
                {
                    var cDate = (c.CreatedAt ?? now).ToUniversalTime().Date;
                    return cDate > startDate && cDate <= endDate;
                }).ToList();

                result.Add(new MonthlyContactsDto
                {
                    Month = 6 - i,
                    MonthLabel = label,
                    TotalMessages = intervalMessages.Count,
                    RepliedMessages = intervalMessages.Count(c => c.Status == "Replied"),
                    UnreadMessages = intervalMessages.Count(c => c.Status == "Unread")
                });
            }
        }
        else if (normalizedPeriod == "yearly")
        {
            int currentYear = now.Year;
            for (int y = currentYear - 3; y <= currentYear; y++)
            {
                var yearMessages = allMessages.Where(c => (c.CreatedAt ?? now).ToUniversalTime().Year == y).ToList();

                result.Add(new MonthlyContactsDto
                {
                    Month = y,
                    MonthLabel = y.ToString(),
                    TotalMessages = yearMessages.Count,
                    RepliedMessages = yearMessages.Count(c => c.Status == "Replied"),
                    UnreadMessages = yearMessages.Count(c => c.Status == "Unread")
                });
            }
        }
        else
        {
            int targetYear = year ?? (int.TryParse(normalizedPeriod, out int parsedY) ? parsedY : now.Year);
            string[] monthNames = { "Thg 1", "Thg 2", "Thg 3", "Thg 4", "Thg 5", "Thg 6", "Thg 7", "Thg 8", "Thg 9", "Thg 10", "Thg 11", "Thg 12" };

            for (int m = 1; m <= 12; m++)
            {
                var monthMessages = allMessages.Where(c =>
                {
                    var cDate = (c.CreatedAt ?? now).ToUniversalTime();
                    return cDate.Year == targetYear && cDate.Month == m;
                }).ToList();

                result.Add(new MonthlyContactsDto
                {
                    Month = m,
                    MonthLabel = monthNames[m - 1],
                    TotalMessages = monthMessages.Count,
                    RepliedMessages = monthMessages.Count(c => c.Status == "Replied"),
                    UnreadMessages = monthMessages.Count(c => c.Status == "Unread")
                });
            }
        }

        return result;
    }

    public async Task<List<TopPerformingPostDto>> GetTopPostsAsync(int limit = 5)
    {
        var posts = await _context.Posts
            .Include(p => p.Category)
            .Include(p => p.Comments)
            .OrderByDescending(p => p.ViewCount)
            .Take(limit)
            .Select(p => new TopPerformingPostDto
            {
                Id = p.Id,
                Title = p.Title,
                Slug = p.Slug,
                CategoryName = p.Category != null ? p.Category.Name : "Chưa phân loại",
                ViewCount = p.ViewCount,
                CommentCount = p.Comments.Count,
                PublishedAt = p.PublishedAt,
                ThumbnailUrl = p.ThumbnailUrl
            })
            .ToListAsync();

        return posts;
    }
}
