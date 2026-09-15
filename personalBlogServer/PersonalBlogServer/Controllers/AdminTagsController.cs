using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PersonalBlogServer.Contracts.Admin;
using PersonalBlogServer.Services.Admin;

namespace PersonalBlogServer.Controllers;

[ApiController]
[Route("api/admin/tags")]
[Authorize]
public class AdminTagsController : ControllerBase
{
    private readonly IAdminTagService _tagService;

    public AdminTagsController(IAdminTagService tagService)
    {
        _tagService = tagService;
    }

    [HttpGet]
    public async Task<IActionResult> GetTags(
        [FromQuery] string? search,
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 10,
        CancellationToken cancellationToken = default)
    {
        var result = await _tagService.GetTagsAsync(search, page, pageSize, cancellationToken);
        return Ok(result);
    }

    [HttpGet("stats")]
    public async Task<IActionResult> GetStats(CancellationToken cancellationToken = default)
    {
        var stats = await _tagService.GetStatsAsync(cancellationToken);
        return Ok(stats);
    }

    [HttpGet("{id:int}")]
    public async Task<IActionResult> GetTagById(int id, CancellationToken cancellationToken = default)
    {
        var tag = await _tagService.GetTagByIdAsync(id, cancellationToken);
        if (tag is null)
        {
            return NotFound(new { message = $"Không tìm thấy thẻ có mã #{id}" });
        }

        return Ok(tag);
    }

    [HttpPost]
    public async Task<IActionResult> CreateTag([FromBody] CreateTagRequest request, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(request.Name))
        {
            return BadRequest(new { message = "Tên thẻ bài viết không được để trống." });
        }

        var created = await _tagService.CreateTagAsync(request, cancellationToken);
        return CreatedAtAction(nameof(GetTagById), new { id = created.Id }, created);
    }

    [HttpPut("{id:int}")]
    public async Task<IActionResult> UpdateTag(int id, [FromBody] UpdateTagRequest request, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(request.Name))
        {
            return BadRequest(new { message = "Tên thẻ bài viết không được để trống." });
        }

        var updated = await _tagService.UpdateTagAsync(id, request, cancellationToken);
        if (updated is null)
        {
            return NotFound(new { message = $"Không tìm thấy thẻ có mã #{id} để cập nhật." });
        }

        return Ok(updated);
    }

    [HttpDelete("{id:int}")]
    public async Task<IActionResult> DeleteTag(int id, CancellationToken cancellationToken = default)
    {
        var success = await _tagService.DeleteTagAsync(id, cancellationToken);
        if (!success)
        {
            return NotFound(new { message = $"Không tìm thấy thẻ có mã #{id} để xóa." });
        }

        return Ok(new { message = "Đã xóa thẻ bài viết thành công." });
    }
}
