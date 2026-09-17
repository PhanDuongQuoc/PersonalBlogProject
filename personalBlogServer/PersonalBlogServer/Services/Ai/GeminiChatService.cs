using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Text.Json.Nodes;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using PersonalBlogServer.Contracts.Ai;
using PersonalBlogServer.Models;
using YamlDotNet.Serialization;
using YamlDotNet.Serialization.NamingConventions;

namespace PersonalBlogServer.Services.Ai;

public class GeminiChatService : IGeminiChatService
{
    private readonly HttpClient _httpClient;
    private readonly AppDbContext _dbContext;
    private readonly IConfiguration _configuration;
    private readonly IWebHostEnvironment _env;
    private readonly ILogger<GeminiChatService> _logger;
    private readonly IDeserializer _yamlDeserializer;

    public GeminiChatService(
        HttpClient httpClient,
        AppDbContext dbContext,
        IConfiguration configuration,
        IWebHostEnvironment env,
        ILogger<GeminiChatService> logger)
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

    public async Task<AiChatResponse> GenerateChatReplyAsync(AiChatRequest request, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(request.Message))
        {
            return new AiChatResponse
            {
                Success = false,
                Error = "Nội dung tin nhắn không được để trống."
            };
        }

        var apiKey = _configuration["Gemini:ApiKey"];
        var model = _configuration["Gemini:Model"] ?? "gemini-2.5-flash";

        if (string.IsNullOrWhiteSpace(apiKey))
        {
            _logger.LogError("Gemini API key is not configured.");
            return new AiChatResponse
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

            // Add current user prompt
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

            var jsonContent = new StringContent(
                JsonSerializer.Serialize(payload),
                Encoding.UTF8,
                "application/json"
            );

            var endpoint = $"https://generativelanguage.googleapis.com/v1beta/models/{model}:generateContent?key={apiKey}";
            var response = await _httpClient.PostAsync(endpoint, jsonContent, cancellationToken);

            var responseBody = await response.Content.ReadAsStringAsync(cancellationToken);

            if (!response.IsSuccessStatusCode)
            {
                _logger.LogError("Gemini API error {StatusCode}: {ResponseBody}", response.StatusCode, responseBody);
                return new AiChatResponse
                {
                    Success = false,
                    Error = "Không thể kết nối đến Gemini AI. Vui lòng thử lại sau giây lát."
                };
            }

            // 4. Parse Gemini JSON response
            var jsonNode = JsonNode.Parse(responseBody);
            var replyText = jsonNode?["candidates"]?[0]?["content"]?[parts()]?[0]?["text"]?.ToString()
                            ?? jsonNode?["candidates"]?[0]?["content"]?["parts"]?[0]?["text"]?.ToString();

            if (string.IsNullOrWhiteSpace(replyText))
            {
                return new AiChatResponse
                {
                    Success = false,
                    Error = "Không nhận được phản hồi hợp lệ từ mô hình AI."
                };
            }

            return new AiChatResponse
            {
                Success = true,
                Reply = replyText.Trim(),
                SuggestedFollowUps = GenerateFollowUpSuggestions(request.Message)
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Exception while processing AI Chat request.");
            return new AiChatResponse
            {
                Success = false,
                Error = "Đã xảy ra lỗi trong quá trình xử lý AI."
            };
        }
    }

    private static string parts() => "parts";

    private async Task<SystemPromptConfig> LoadPromptConfigAsync(CancellationToken cancellationToken)
    {
        var promptPath = Path.Combine(_env.ContentRootPath, "Prompts", "system_prompt.yaml");
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
                _logger.LogWarning(ex, "Failed to parse system_prompt.yaml, using fallback prompt config.");
            }
        }

        // Fallback default prompt config if file not found
        return new SystemPromptConfig
        {
            Metadata = new PromptMetadata { BotName = "PDQ AI Assistant" },
            Parameters = new PromptParameters { Temperature = 0.7, TopP = 0.9, MaxOutputTokens = 1200 },
            Persona = new PromptPersona
            {
                Role = "Trợ lý thông minh chính thức của Blog & Portfolio cá nhân Phan Dương Quốc",
                Tone = "Lịch sự, thân thiện, khiêm tốn, chuyên nghiệp và nhiệt tình",
                FormatStyle = "Markdown chuẩn mực, có tiêu đề, bullet points, in đậm từ khóa"
            },
            Rules = new List<string>
            {
                "Luôn ưu tiên trả lời bằng tiếng Việt trừ khi người dùng hỏi bằng tiếng Anh.",
                "Cung cấp thông tin liên hệ và link bài viết chính xác."
            },
            Template = "Bạn là {{bot_name}} - {{persona_role}}.\nPhong cách: {{persona_tone}}.\n### THÔNG TIN TÁC GIẢ:\n{{author_profile}}\n### DANH SÁCH BÀI VIẾT:\n{{blog_posts}}\n### NGUYÊN TẮC:\n{{rules}}"
        };
    }

    private async Task<string> RenderSystemInstructionAsync(SystemPromptConfig config, CancellationToken cancellationToken)
    {
        // 1. Fetch Author Profile from DB
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

        // 2. Fetch Latest Published Blog Posts from DB
        var publishedPosts = await _dbContext.Posts
            .Include(p => p.Category)
            .Include(p => p.Tags)
            .Where(p => p.Status == "Published")
            .OrderByDescending(p => p.PublishedAt ?? p.CreatedAt)
            .Take(15)
            .ToListAsync(cancellationToken);

        var postsSb = new StringBuilder();
        if (publishedPosts.Any())
        {
            foreach (var post in publishedPosts)
            {
                var cat = post.Category?.Name ?? "General";
                var tags = post.Tags.Any() ? string.Join(", ", post.Tags.Select(t => t.Name)) : "N/A";
                postsSb.AppendLine($"- Tiêu đề: \"{post.Title}\" | Chuyên mục: [{cat}] | Thẻ: [{tags}] | Slug link: /#/posts/{post.Slug}");
                if (!string.IsNullOrWhiteSpace(post.Excerpt))
                {
                    postsSb.AppendLine($"  Tóm tắt: {post.Excerpt}");
                }
            }
        }
        else
        {
            postsSb.AppendLine("- Hiện blog có các chủ đề về Clean Architecture, Vue.js, .NET 9, Tối ưu hóa hiệu năng cơ sở dữ liệu.");
        }

        // 3. Format Rules
        var rulesSb = new StringBuilder();
        var index = 1;
        foreach (var rule in config.Rules)
        {
            rulesSb.AppendLine($"{index++}. {rule}");
        }

        // 4. Render Template Placeholders
        var rendered = config.Template
            .Replace("{{bot_name}}", config.Metadata.BotName)
            .Replace("{{persona_role}}", config.Persona.Role)
            .Replace("{{persona_tone}}", config.Persona.Tone)
            .Replace("{{persona_format_style}}", config.Persona.FormatStyle)
            .Replace("{{author_profile}}", authorSb.ToString().TrimEnd())
            .Replace("{{blog_posts}}", postsSb.ToString().TrimEnd())
            .Replace("{{rules}}", rulesSb.ToString().TrimEnd());

        return rendered;
    }

    private static List<string> GenerateFollowUpSuggestions(string userMessage)
    {
        var lower = userMessage.ToLower();
        if (lower.Contains("kinh nghiệm") || lower.Contains("kỹ năng") || lower.Contains("skill"))
        {
            return new List<string>
            {
                "Quốc đã từng thực hiện những dự án nào?",
                "Các bài viết chuyên sâu về .NET & Vue.js?",
                "Thông tin liên hệ và CV của tác giả"
            };
        }

        if (lower.Contains("bài viết") || lower.Contains("blog") || lower.Contains("chủ đề"))
        {
            return new List<string>
            {
                "Bài viết mới nhất về Clean Architecture",
                "Kỹ năng lập trình chính của tác giả",
                "Làm sao để gửi bình luận trên blog?"
            };
        }

        return new List<string>
        {
            "Giới thiệu kinh nghiệm của Phan Dương Quốc",
            "Gợi ý bài viết hay về .NET 9 & Vue.js",
            "Làm thế nào để liên hệ hợp tác?"
        };
    }
}
