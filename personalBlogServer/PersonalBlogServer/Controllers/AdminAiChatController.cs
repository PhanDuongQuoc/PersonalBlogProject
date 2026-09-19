using Microsoft.AspNetCore.Mvc;
using PersonalBlogServer.Contracts.Ai;
using PersonalBlogServer.Services.Ai;

namespace PersonalBlogServer.Controllers;

[ApiController]
[Route("api/admin/ai")]
[Route("api/public/adminai")]
public class AdminAiChatController : ControllerBase
{
    private readonly IGeminiAdminChatService _adminAiService;
    public AdminAiChatController(IGeminiAdminChatService adminAiService)
    {
        _adminAiService = adminAiService;
    }

    [HttpPost("chat")]
    [HttpPost("AdminChat")]
    public async Task<IActionResult> AdminChat([FromBody] AdminAiChatRequest request, CancellationToken cancellationToken = default)
    {
        if (request == null || string.IsNullOrWhiteSpace(request.Message))
        {
            return BadRequest(new AdminAiChatResponse
            {
                Success = false,
                Error = "Tin nhắn không được để trống."
            });
        }

        // Limit message length to avoid token abuse
        if (request.Message.Length > 1000)
        {
            return BadRequest(new AdminAiChatResponse
            {
                Success = false,
                Error = "Tin nhắn quá dài (tối đa 1000 ký tự)."
            });
        }

        var response = await _adminAiService.GenerateChatReplyAsync(request, cancellationToken);
        if (!response.Success)
        {
            return StatusCode(500, response);
        }

        return Ok(response);
    }
}
