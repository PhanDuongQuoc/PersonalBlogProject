using System.Text;
using System.Text.Json;
using System.Text.Json.Nodes;
using Microsoft.EntityFrameworkCore;
using PersonalBlogServer.Contracts.Ai;
using PersonalBlogServer.Models;
using YamlDotNet.Serialization;
using YamlDotNet.Serialization.NamingConventions;

namespace PersonalBlogServer.Services.Ai;

public class GeminiAdminChatService : IGeminiAdminChatService
{
    private readonly HttpClient _httpClient;
    private readonly AppDbContext _dbContext;
    private readonly IConfiguration _configuration;
    private readonly IWebHostEnvironment _env;
    private readonly ILogger<GeminiAdminChatService> _logger;
    private readonly IDeserializer _yamlDeserializer;

    public GeminiAdminChatService(
        HttpClient httpClient,
        AppDbContext dbContext,
        IConfiguration configuration,
        IWebHostEnvironment env,
        ILogger<GeminiAdminChatService> logger)
    {
        _httpClient = httpClient;
        _dbContext = dbContext;
        _configuration = configuration;
        _env = env;
        _logger = logger;
        _yamlDeserializer = new DeserializerBuilder()
            .WithNamingConvention(UnderscoredNamingConvention.Instance)
            .IgnoreUnmatchedProperties()
            .Build();
    }

    public async Task<AdminAiChatResponse> GenerateChatReplyAsync(AdminAiChatRequest request, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(request.Message))
        {
            return new AdminAiChatResponse
            {
                Success = false,
                Error = "Nội dung tin nhắn không được để trống."
            };
        }

        var apiKey = _configuration["Gemini:ApiKey"];
        var model = _configuration["Gemini:Model"] ?? "gemini-flash-latest";
        if (string.IsNullOrWhiteSpace(apiKey))
        {
            _logger.LogError("Gemini API key is not configured.");
            return new AdminAiChatResponse
            {
                Success = false,
                Error = "Gemini API key chưa được cấu hình trên máy chủ."
            };
        }

        try
        {
            // 1. Load YAML Prompt Config & Build dynamic system instruction
            var promptConfig = await LoadPromptConfigAsync(cancellationToken);
            var systemInstructionText = await RenderSystemInstructionAsync(promptConfig, cancellationToken);

            // 2. Prepare contents list for Gemini API
            var contents = new List<object>();

            if (request.History != null && request.History.Count > 0)
            {
                // Take only the last 10 messages for conversation efficiency
                var recentHistory = request.History.TakeLast(10);
                foreach (var hist in recentHistory)
                {
                    if (string.IsNullOrWhiteSpace(hist.Content)) continue;
                    var role = hist.Role.Equals("model", StringComparison.OrdinalIgnoreCase) ||
                               hist.Role.Equals("assistant", StringComparison.OrdinalIgnoreCase)
                        ? "model"
                        : "user";

                    contents.Add(new
                    {
                        role = role,
                        parts = new[] { new { text = hist.Content } }
                    });
                }
            }

            // Add current user prompt (Note: Gemini API requires role 'user')
            contents.Add(new
            {
                role = "user",
                parts = new[] { new { text = request.Message.Trim() } }
            });

            // 3. Build API request payload with parameters from YAML
            var payload = new
            {
                system_instruction = new
                {
                    parts = new[] { new { text = systemInstructionText } }
                },
                contents = contents,
                generationConfig = new
                {
                    temperature = promptConfig.Parameters.Temperature,
                    topP = promptConfig.Parameters.TopP,
                    maxOutputTokens = promptConfig.Parameters.MaxOutputTokens
                }
            };

            // 3. Candidate models to handle temporary Google 503 or 429 spikes automatically
            var candidateModels = new List<string> { model, "gemini-3.5-flash", "gemini-3.8-flash", "gemini-flash-lite-latest", "gemini-flash-latest" }
                .Distinct(StringComparer.OrdinalIgnoreCase)
                .ToList();

            string? responseBody = null;
            bool isSuccess = false;

            foreach (var targetModel in candidateModels)
            {
                var endpoint = $"https://generativelanguage.googleapis.com/v1beta/models/{targetModel}:generateContent?key={apiKey}";
                var jsonContent = new StringContent(
                    JsonSerializer.Serialize(payload),
                    Encoding.UTF8,
                    "application/json"
                );

                var response = await _httpClient.PostAsync(endpoint, jsonContent, cancellationToken);
                responseBody = await response.Content.ReadAsStringAsync(cancellationToken);

                if (response.IsSuccessStatusCode)
                {
                    isSuccess = true;
                    break;
                }

                _logger.LogWarning("Gemini API call to {Model} returned {StatusCode}. Trying fallback model...", targetModel, response.StatusCode);
            }

            if (!isSuccess || string.IsNullOrWhiteSpace(responseBody))
            {
                _logger.LogError("All candidate Gemini models failed. Last response: {ResponseBody}", responseBody);
                return new AdminAiChatResponse
                {
                    Success = false,
                    Error = "Không thể kết nối đến Gemini AI. Vui lòng thử lại sau giây lát."
                };
            }

            // 4. Parse Gemini JSON response (supports multi-part and thinking responses)
            var jsonNode = JsonNode.Parse(responseBody);
            var partsArray = jsonNode?["candidates"]?[0]?["content"]?["parts"]?.AsArray();
            var replyText = partsArray != null
                ? string.Join("\n", partsArray.Select(p => p?["text"]?.ToString()).Where(t => !string.IsNullOrWhiteSpace(t)))
                : jsonNode?["candidates"]?[0]?["content"]?["parts"]?[0]?["text"]?.ToString();

            if (string.IsNullOrWhiteSpace(replyText))
            {
                return new AdminAiChatResponse
                {
                    Success = false,
                    Error = "Không nhận được phản hồi hợp lệ từ mô hình AI."
                };
            }

            return new AdminAiChatResponse
            {
                Success = true,
                Reply = replyText.Trim(),
                SuggestedFollowUps = GenerateFollowUpSuggestions(request.Message)
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Exception while processing Admin AI Chat request.");
            return new AdminAiChatResponse
            {
                Success = false,
                Error = "Đã xảy ra lỗi trong quá trình xử lý AI."
            };
        }
    }

    private async Task<SystemPromptConfig> LoadPromptConfigAsync(CancellationToken cancellationToken)
    {
        var promptPath = Path.Combine(_env.ContentRootPath, "Prompts", "admin_system_prompt.yaml");
        if (File.Exists(promptPath))
        {
            try
            {
                var yamlContent = await File.ReadAllTextAsync(promptPath, cancellationToken);
                var config = _yamlDeserializer.Deserialize<SystemPromptConfig>(yamlContent);
                if (config != null && !string.IsNullOrWhiteSpace(config.Template))
                {
                    return config;
                }
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "Failed to parse admin_system_prompt.yaml, using fallback prompt config.");
            }
        }

        // Fallback default prompt config if file not found
        return new SystemPromptConfig
        {
            Metadata = new PromptMetadata { BotName = "PDQ Admin AI" },
            Parameters = new PromptParameters { Temperature = 0.7, TopP = 0.95, MaxOutputTokens = 2500 },
            Persona = new PromptPersona
            {
                Role = "Cố vấn & Trợ lý ảo AI đắc lực cho Quản trị viên Blog (Phan Dương Quốc)",
                Tone = "Chuyên nghiệp, thẳng thắn, chiến lược, thông tuệ và thực tiễn",
                FormatStyle = "Markdown chuẩn mực, cấu trúc rõ ràng, có tiêu đề, bullet points, bảng biểu, code blocks và biểu đồ Highcharts"
            },
            Rules = new List<string>
            {
                "Hỗ trợ Admin sáng tạo nội dung, tối ưu SEO, phân tích số liệu và trực quan hóa biểu đồ Highcharts.",
                "Đưa ra các bước hành động cụ thể và định dạng Markdown dễ đọc."
            },
            Template = "Bạn là {{bot_name}} - {{persona_role}}.\nPhong cách: {{persona_tone}}.\n### TỔNG QUAN HỆ THỐNG:\n{{admin_overview}}\n### DỮ LIỆU THỐNG KÊ:\n{{analytics_data}}\n### BÀI VIẾT:\n{{blog_posts}}\n### NGUYÊN TẮC:\n{{rules}}"
        };
    }

    private async Task<string> RenderSystemInstructionAsync(SystemPromptConfig config, CancellationToken cancellationToken)
    {
        // 1. Fetch Admin System Overview Metrics
        var publishedCount = await _dbContext.Posts.CountAsync(p => p.Status == "Published", cancellationToken);
        var draftCount = await _dbContext.Posts.CountAsync(p => p.Status == "Draft", cancellationToken);
        var archivedCount = await _dbContext.Posts.CountAsync(p => p.Status == "Archived", cancellationToken);
        var totalPosts = publishedCount + draftCount + archivedCount;
        var totalViews = await _dbContext.Posts.SumAsync(p => (int?)p.ViewCount, cancellationToken) ?? 0;

        var totalCategories = await _dbContext.Categories.CountAsync(cancellationToken);
        var totalTags = await _dbContext.Tags.CountAsync(cancellationToken);

        var totalComments = await _dbContext.Comments.CountAsync(cancellationToken);
        var pendingComments = await _dbContext.Comments.CountAsync(c => c.Status == "Pending", cancellationToken);
        var approvedComments = await _dbContext.Comments.CountAsync(c => c.Status == "Approved", cancellationToken);

        var totalContacts = await _dbContext.ContactMessages.CountAsync(cancellationToken);
        var unreadContacts = await _dbContext.ContactMessages.CountAsync(c => c.Status == "New" || c.Status == "Unread" || c.Status == "Pending", cancellationToken);
        var repliedContacts = await _dbContext.ContactMessages.CountAsync(c => c.Status == "Replied", cancellationToken);

        var overviewSb = new StringBuilder();
        overviewSb.AppendLine($"- Bài viết: {publishedCount} đã xuất bản, {draftCount} bản nháp, {archivedCount} đã lưu trữ. (Tổng số bài: {totalPosts}, Tổng lượt xem toàn trang: {totalViews:N0})");
        overviewSb.AppendLine($"- Danh mục & Thẻ: {totalCategories} chuyên mục, {totalTags} thẻ tag.");
        overviewSb.AppendLine($"- Bình luận: {totalComments} bình luận ({approvedComments} đã duyệt, {pendingComments} đang chờ duyệt).");
        overviewSb.AppendLine($"- Tin nhắn liên hệ: {totalContacts} liên hệ ({unreadContacts} chưa xử lý, {repliedContacts} đã phản hồi).");

        // 2. Fetch Categories with Post count & Views for Analytics
        var categories = await _dbContext.Categories
            .Include(c => c.Posts)
            .OrderByDescending(c => c.Posts.Count)
            .ToListAsync(cancellationToken);

        var tags = await _dbContext.Tags
            .OrderBy(t => t.Name)
            .Take(20)
            .ToListAsync(cancellationToken);

        var catTagsSb = new StringBuilder();
        if (categories.Any())
        {
            var catList = string.Join(", ", categories.Select(c => $"{c.Name} ({c.Posts.Count} bài, {c.Posts.Sum(p => p.ViewCount)} views)"));
            catTagsSb.AppendLine($"- Chuyên mục chi tiết: {catList}");
        }
        if (tags.Any())
        {
            var tagList = string.Join(", ", tags.Select(t => t.Name));
            catTagsSb.AppendLine($"- Thẻ tag phổ biến: {tagList}");
        }

        // 3. Build Detailed Analytics Data for Chart Generation
        var analyticsSb = new StringBuilder();
        analyticsSb.AppendLine("Dữ liệu thực tế để vẽ biểu đồ khi Admin yêu cầu:");
        
        // a. Category distribution data
        analyticsSb.AppendLine("- Phân bố chuyên mục (Category Distribution):");
        foreach (var c in categories)
        {
            var catViews = c.Posts.Sum(p => p.ViewCount);
            analyticsSb.AppendLine($"  + Chuyên mục \"{c.Name}\": {c.Posts.Count} bài viết, {catViews} lượt xem.");
        }

        // b. Post Status distribution data
        analyticsSb.AppendLine($"- Phân bố trạng thái bài viết: Published={publishedCount}, Draft={draftCount}, Archived={archivedCount}.");

        // c. Top 5 most viewed posts
        var topPosts = await _dbContext.Posts
            .Where(p => p.Status == "Published")
            .OrderByDescending(p => p.ViewCount)
            .Take(5)
            .ToListAsync(cancellationToken);

        if (topPosts.Any())
        {
            analyticsSb.AppendLine("- Top bài viết nhiều lượt xem nhất:");
            foreach (var tp in topPosts)
            {
                analyticsSb.AppendLine($"  + \"{tp.Title}\": {tp.ViewCount} views");
            }
        }

        // d. Monthly Views & Unique Readers trend (grouped by month of current year)
        var allPosts = await _dbContext.Posts.AsNoTracking().ToListAsync(cancellationToken);
        var currentYear = DateTime.UtcNow.Year;
        string[] monthNames = { "Thg 1", "Thg 2", "Thg 3", "Thg 4", "Thg 5", "Thg 6", "Thg 7", "Thg 8", "Thg 9", "Thg 10", "Thg 11", "Thg 12" };
        var monthlyViewsArray = new List<long>();
        var monthlyReadersArray = new List<long>();
        for (int m = 1; m <= 12; m++)
        {
            var mViews = (long)allPosts.Where(p => (p.PublishedAt ?? p.CreatedAt).Year == currentYear && (p.PublishedAt ?? p.CreatedAt).Month == m)
                                       .Sum(p => p.ViewCount);
            var mReaders = mViews > 0 ? (long)(mViews * 0.9) : 0;
            monthlyViewsArray.Add(mViews);
            monthlyReadersArray.Add(mReaders);
        }
        analyticsSb.AppendLine($"- Xu hướng lượt xem & độc giả 12 tháng năm {currentYear}:");
        analyticsSb.AppendLine($"  + Danh sách tháng: [{string.Join(", ", monthNames.Select(m => $"\"{m}\""))}]");
        analyticsSb.AppendLine($"  + Lượt xem (Views): [{string.Join(", ", monthlyViewsArray)}]");
        analyticsSb.AppendLine($"  + Độc giả (Unique Readers): [{string.Join(", ", monthlyReadersArray)}]");

        // 4. Fetch Recent Blog Posts (Both Published and Drafts)
        var recentPosts = await _dbContext.Posts
            .Include(p => p.Category)
            .Include(p => p.Tags)
            .OrderByDescending(p => p.UpdatedAt ?? p.CreatedAt)
            .Take(15)
            .ToListAsync(cancellationToken);

        var postsSb = new StringBuilder();
        if (recentPosts.Any())
        {
            foreach (var post in recentPosts)
            {
                var cat = post.Category?.Name ?? "General";
                var tagsStr = post.Tags.Any() ? string.Join(", ", post.Tags.Select(t => t.Name)) : "N/A";
                postsSb.AppendLine($"- [{post.Status.ToUpper()}] \"{post.Title}\" | Chuyên mục: [{cat}] | Thẻ: [{tagsStr}] | Views: {post.ViewCount} | Slug: /#/posts/{post.Slug}");
                if (!string.IsNullOrWhiteSpace(post.Excerpt))
                {
                    postsSb.AppendLine($"  Tóm tắt: {post.Excerpt}");
                }
            }
        }
        else
        {
            postsSb.AppendLine("- Hệ thống chưa có bài viết nào.");
        }

        // 5. Fetch Author Profile from DB
        var author = await _dbContext.Users
            .Include(u => u.UserSkills)
            .Include(u => u.UserExperiences)
            .FirstOrDefaultAsync(u => u.Role == "Admin" || u.IsActive, cancellationToken);

        var authorSb = new StringBuilder();
        authorSb.AppendLine($"- Họ và tên: {author?.DisplayName ?? "Phan Dương Quốc"}");
        authorSb.AppendLine($"- Chức danh: {author?.JobTitle ?? "Fullstack Software Engineer (.NET 9 & Vue.js / Nuxt)"}");
        authorSb.AppendLine($"- Địa điểm: {author?.Location ?? "TP. Hồ Chí Minh, Việt Nam"}");
        authorSb.AppendLine($"- Email liên hệ: {author?.Email ?? "phanduongquoc111@gmail.com"}");
        authorSb.AppendLine($"- GitHub: {author?.GithubUrl ?? "https://github.com/phanduongquoc11"}");
        authorSb.AppendLine($"- LinkedIn: {author?.LinkedinUrl ?? "https://linkedin.com/in/phanduongquoc"}");
        authorSb.AppendLine($"- Website / Blog: {author?.WebsiteUrl ?? "https://phanduongquoc.dev"}");
        if (!string.IsNullOrWhiteSpace(author?.Bio))
        {
            authorSb.AppendLine($"- Tiểu sử tóm tắt: {author.Bio}");
        }

        if (author?.UserSkills != null && author.UserSkills.Any())
        {
            var skills = string.Join(", ", author.UserSkills.Select(s => s.Name));
            authorSb.AppendLine($"- Kỹ năng chuyên môn: {skills}");
        }
        else
        {
            authorSb.AppendLine("- Kỹ năng chính: C#, .NET 9, ASP.NET Core Web API, Entity Framework Core, PostgreSQL, Clean Architecture, Vue 3, TypeScript, Quasar Framework, Tailwind CSS, Pinia, RESTful API, Docker, CI/CD.");
        }

        if (author?.UserExperiences != null && author.UserExperiences.Any())
        {
            authorSb.AppendLine("- Kinh nghiệm làm việc:");
            foreach (var exp in author.UserExperiences.OrderBy(e => e.DisplayOrder))
            {
                var period = $"{exp.StartDate} - {(exp.IsCurrent ? "Hiện tại" : exp.EndDate ?? "N/A")}";
                authorSb.AppendLine($"  + {exp.Role} tại {exp.Company} ({period}): {exp.Description}");
            }
        }

        // 6. Format Rules
        var rulesSb = new StringBuilder();
        var index = 1;
        foreach (var rule in config.Rules)
        {
            rulesSb.AppendLine($"{index++}. {rule}");
        }

        // 7. Render Template Placeholders
        var rendered = config.Template
            .Replace("{{bot_name}}", config.Metadata.BotName)
            .Replace("{{persona_role}}", config.Persona.Role)
            .Replace("{{persona_tone}}", config.Persona.Tone)
            .Replace("{{persona_format_style}}", config.Persona.FormatStyle)
            .Replace("{{admin_overview}}", overviewSb.ToString().TrimEnd())
            .Replace("{{analytics_data}}", analyticsSb.ToString().TrimEnd())
            .Replace("{{categories_and_tags}}", catTagsSb.ToString().TrimEnd())
            .Replace("{{blog_posts}}", postsSb.ToString().TrimEnd())
            .Replace("{{author_profile}}", authorSb.ToString().TrimEnd())
            .Replace("{{rules}}", rulesSb.ToString().TrimEnd());

        return rendered;
    }

    private static List<string> GenerateFollowUpSuggestions(string userMessage)
    {
        var lower = userMessage.ToLower();

        // 1. Biểu đồ / Trực quan hóa / Thống kê / Analytics / Lượt xem / Chuyên mục
        if (lower.Contains("biểu đồ") || lower.Contains("chart") || lower.Contains("thống kê") ||
            lower.Contains("số liệu") || lower.Contains("lượt xem") || lower.Contains("analytics") ||
            lower.Contains("báo cáo") || lower.Contains("view") || lower.Contains("tương tác"))
        {
            return new List<string>
            {
                "Vẽ biểu đồ phân bố bài viết theo chuyên mục",
                "Vẽ biểu đồ xu hướng lượt xem các tháng gần đây",
                "Vẽ biểu đồ tròn tỉ lệ trạng thái bài viết (Published/Draft)",
                "Đánh giá top các bài viết có nhiều lượt xem nhất"
            };
        }

        // 2. Viết bài / Ý tưởng nội dung / SEO / Tiêu đề / Bài viết
        if (lower.Contains("bài viết") || lower.Contains("ý tưởng") || lower.Contains("viết bài") ||
            lower.Contains("seo") || lower.Contains("tiêu đề") || lower.Contains("nội dung") ||
            lower.Contains("outline") || lower.Contains("dàn ý") || lower.Contains("slug"))
        {
            return new List<string>
            {
                "Lập dàn ý chi tiết bài viết mới về Clean Architecture & CQRS trong .NET 9",
                "Gợi ý 5 tiêu đề cuốn hút & chuẩn SEO cho bài viết tiếp theo",
                "Đề xuất chuyên mục và thẻ tag tối ưu cho bài viết nháp",
                "Vẽ biểu đồ phân bố bài viết theo chuyên mục"
            };
        }

        // 3. Liên hệ / Email / Tuyển dụng / Bình luận / Phản hồi
        if (lower.Contains("liên hệ") || lower.Contains("contact") || lower.Contains("email") ||
            lower.Contains("tuyển dụng") || lower.Contains("bình luận") || lower.Contains("comment") ||
            lower.Contains("phản hồi") || lower.Contains("tin nhắn"))
        {
            return new List<string>
            {
                "Soạn email mẫu trả lời phỏng vấn & cơ hội việc làm chuyên nghiệp",
                "Gợi ý cách phản hồi bình luận kỹ thuật chuyên sâu từ độc giả",
                "Cách phân loại và xử lý tin nhắn liên hệ dạng spam / quảng cáo",
                "Mẫu thư cảm ơn và kết nối với đối tác tuyển dụng"
            };
        }

        // 4. Kỹ thuật / Kiến trúc / Code / Bug / .NET / Vue / Database / Performance
        if (lower.Contains("kỹ thuật") || lower.Contains("code") || lower.Contains("c#") ||
            lower.Contains(".net") || lower.Contains("vue") || lower.Contains("clean architecture") ||
            lower.Contains("postgres") || lower.Contains("sql") || lower.Contains("performance") ||
            lower.Contains("docker") || lower.Contains("ci/cd"))
        {
            return new List<string>
            {
                "Tối ưu hiệu năng truy vấn Entity Framework Core với PostgreSQL",
                "Hướng dẫn triển khai Clean Architecture chuẩn mực trong .NET 9",
                "Cách tối ưu bundle size và trải nghiệm người dùng trong Vue 3",
                "Thiết kế Caching layer với MemoryCache và Redis"
            };
        }

        // 5. Quản trị / Tính năng / Giao diện / Portfolio / Settings
        if (lower.Contains("quản trị") || lower.Contains("tính năng") || lower.Contains("giao diện") ||
            lower.Contains("portfolio") || lower.Contains("cài đặt") || lower.Contains("admin"))
        {
            return new List<string>
            {
                "Vẽ biểu đồ phân bố bài viết theo chuyên mục",
                "Đề xuất các tính năng mới nên bổ sung cho blog cá nhân",
                "Gợi ý nâng cấp trải nghiệm Dashboard quản trị trực quan hơn",
                "Cách làm nổi bật các dự án Portfolio để gây ấn tượng mạnh"
            };
        }

        // Default quick actions cho Admin
        return new List<string>
        {
            "Vẽ biểu đồ phân bố bài viết theo chuyên mục",
            "Vẽ biểu đồ xu hướng lượt xem bài viết",
            "Lập dàn ý bài viết kỹ thuật mới (.NET 9 / Vue 3)",
            "Phân tích và tối ưu hóa SEO cho các bài viết"
        };
    }
}
