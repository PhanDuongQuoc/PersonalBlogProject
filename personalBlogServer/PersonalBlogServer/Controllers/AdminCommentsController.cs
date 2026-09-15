using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PersonalBlogServer.Contracts.Admin;
using PersonalBlogServer.Services.Admin;

namespace PersonalBlogServer.Controllers;

[ApiController]
[Route("api/admin/comments")]
[Authorize]
public class AdminCommentsController : ControllerBase
{
    private readonly IAdminCommentService _commentService;

    public AdminCommentsController(IAdminCommentService commentService)
    {
        _commentService = commentService;
    }

    [HttpGet]
    public async Task<IActionResult> GetComments(
        [FromQuery] string? search,
        [FromQuery] string? status,
        [FromQuery] int? postId,
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 10,
        CancellationToken cancellationToken = default)
    {
        var result = await _commentService.GetCommentsAsync(search, status, postId, page, pageSize, cancellationToken);
        return Ok(result);
    }

    [HttpGet("stats")]
    public async Task<IActionResult> GetStats(CancellationToken cancellationToken = default)
    {
        var stats = await _commentService.GetStatsAsync(cancellationToken);
        return Ok(stats);
    }

    [HttpGet("{id:int}")]
    public async Task<IActionResult> GetCommentById(int id, CancellationToken cancellationToken = default)
    {
        var comment = await _commentService.GetCommentByIdAsync(id, cancellationToken);
        if (comment is null)
        {
            return NotFound(new { message = $"Không tìm thấy bình luận #{id}" });
        }

        return Ok(comment);
    }

    [HttpPatch("{id:int}/status")]
    public async Task<IActionResult> UpdateStatus(int id, [FromBody] UpdateCommentStatusRequest request, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(request.Status))
        {
            return BadRequest(new { message = "Trạng thái bình luận không hợp lệ." });
        }

        var success = await _commentService.UpdateStatusAsync(id, request.Status, cancellationToken);
        if (!success)
        {
            return NotFound(new { message = $"Không tìm thấy bình luận #{id} để cập nhật trạng thái." });
        }

        return Ok(new { message = "Đã cập nhật trạng thái bình luận thành công." });
    }

    [HttpDelete("{id:int}")]
    public async Task<IActionResult> DeleteComment(int id, CancellationToken cancellationToken = default)
    {
        var success = await _commentService.DeleteCommentAsync(id, cancellationToken);
        if (!success)
        {
            return NotFound(new { message = $"Không tìm thấy bình luận #{id} để xóa." });
        }

        return Ok(new { message = "Đã xóa bình luận thành công." });
    }
}
