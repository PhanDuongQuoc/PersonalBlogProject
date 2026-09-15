using System;
using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PersonalBlogServer.Contracts.Admin;
using PersonalBlogServer.Services.Admin;

namespace PersonalBlogServer.Controllers;

[ApiController]
[Route("api/admin/posts")]
[Authorize]
public class AdminPostsController : ControllerBase
{
    private readonly IAdminPostService _postService;

    public AdminPostsController(IAdminPostService postService)
    {
        _postService = postService;
    }

    [HttpGet]
    public async Task<IActionResult> GetPosts(
        [FromQuery] string? search,
        [FromQuery] int? categoryId,
        [FromQuery] string? status,
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 10,
        CancellationToken cancellationToken = default)
    {
        var result = await _postService.GetPostsAsync(search, categoryId, status, page, pageSize, cancellationToken);
        return Ok(result);
    }

    [HttpGet("stats")]
    public async Task<IActionResult> GetStats(CancellationToken cancellationToken = default)
    {
        var stats = await _postService.GetStatsAsync(cancellationToken);
        return Ok(stats);
    }

    [HttpGet("metadata")]
    public async Task<IActionResult> GetMetadata(CancellationToken cancellationToken = default)
    {
        var categories = await _postService.GetCategoriesAsync(cancellationToken);
        var tags = await _postService.GetTagsAsync(cancellationToken);

        return Ok(new
        {
            Categories = categories,
            Tags = tags
        });
    }

    [HttpGet("{id:int}")]
    public async Task<IActionResult> GetPostById(int id, CancellationToken cancellationToken = default)
    {
        var post = await _postService.GetPostByIdAsync(id, cancellationToken);
        if (post is null)
        {
            return NotFound(new { message = $"Không tìm thấy bài viết có mã #{id}" });
        }

        return Ok(post);
    }

    [HttpPost]
    public async Task<IActionResult> CreatePost([FromBody] CreatePostRequest request, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(request.Title))
        {
            return BadRequest(new { message = "Tiêu đề bài viết không được để trống." });
        }

        if (request.CategoryId <= 0)
        {
            return BadRequest(new { message = "Vui lòng chọn danh mục cho bài viết." });
        }

        var userIdClaim = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (!int.TryParse(userIdClaim, out var authorId))
        {
            return Unauthorized(new { message = "Không xác định được danh tính người dùng từ phiên đăng nhập." });
        }

        var created = await _postService.CreatePostAsync(authorId, request, cancellationToken);
        return CreatedAtAction(nameof(GetPostById), new { id = created.Id }, created);
    }

    [HttpPut("{id:int}")]
    public async Task<IActionResult> UpdatePost(int id, [FromBody] UpdatePostRequest request, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(request.Title))
        {
            return BadRequest(new { message = "Tiêu đề bài viết không được để trống." });
        }

        if (request.CategoryId <= 0)
        {
            return BadRequest(new { message = "Vui lòng chọn danh mục cho bài viết." });
        }

        var updated = await _postService.UpdatePostAsync(id, request, cancellationToken);
        if (updated is null)
        {
            return NotFound(new { message = $"Không tìm thấy bài viết có mã #{id} để cập nhật." });
        }

        return Ok(updated);
    }

    [HttpPatch("{id:int}/status")]
    public async Task<IActionResult> UpdateStatus(int id, [FromBody] UpdatePostStatusRequest request, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(request.Status))
        {
            return BadRequest(new { message = "Trạng thái không hợp lệ." });
        }

        var success = await _postService.UpdateStatusAsync(id, request.Status, cancellationToken);
        if (!success)
        {
            return NotFound(new { message = $"Không tìm thấy bài viết #{id} để đổi trạng thái." });
        }

        return Ok(new { message = "Cập nhật trạng thái bài viết thành công." });
    }

    [HttpDelete("{id:int}")]
    public async Task<IActionResult> DeletePost(int id, CancellationToken cancellationToken = default)
    {
        var success = await _postService.DeletePostAsync(id, cancellationToken);
        if (!success)
        {
            return NotFound(new { message = $"Không tìm thấy bài viết #{id} để xóa." });
        }

        return Ok(new { message = "Đã xóa bài viết thành công." });
    }
}
