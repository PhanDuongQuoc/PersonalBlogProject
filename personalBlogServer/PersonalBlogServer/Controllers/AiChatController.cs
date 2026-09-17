using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PersonalBlogServer.Contracts.Ai;
using PersonalBlogServer.Services.Ai;

namespace PersonalBlogServer.Controllers;

[ApiController]
[Route("api/public/ai")]
[AllowAnonymous]
public class AiChatController : ControllerBase
{
    private readonly IGeminiChatService _aiService;

    public AiChatController(IGeminiChatService aiService)
    {
        _aiService = aiService;
    }

    [HttpPost("chat")]
    public async Task<IActionResult> Chat([FromBody] AiChatRequest request, CancellationToken cancellationToken = default)
    {
        if (request == null || string.IsNullOrWhiteSpace(request.Message))
        {
            return BadRequest(new AiChatResponse
            {
                Success = false,
                Error = "Tin nhắn không được để trống."
            });
        }

        // Limit message length to avoid token abuse
        if (request.Message.Length > 1000)
        {
            return BadRequest(new AiChatResponse
            {
                Success = false,
                Error = "Tin nhắn quá dài (tối đa 1000 ký tự)."
            });
        }

        var response = await _aiService.GenerateChatReplyAsync(request, cancellationToken);
        if (!response.Success)
        {
            return StatusCode(500, response);
        }

        return Ok(response);
    }
}
