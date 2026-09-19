using PersonalBlogServer.Contracts.Ai;

namespace PersonalBlogServer.Services.Ai;

public interface IGeminiAdminChatService
{
    Task<AdminAiChatResponse> GenerateChatReplyAsync(AdminAiChatRequest request, CancellationToken cancellationToken = default);
}
