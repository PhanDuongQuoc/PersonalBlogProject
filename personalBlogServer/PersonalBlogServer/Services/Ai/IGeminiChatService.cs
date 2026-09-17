using System.Threading;
using System.Threading.Tasks;
using PersonalBlogServer.Contracts.Ai;

namespace PersonalBlogServer.Services.Ai;

public interface IGeminiChatService
{
    Task<AiChatResponse> GenerateChatReplyAsync(AiChatRequest request, CancellationToken cancellationToken = default);
}
