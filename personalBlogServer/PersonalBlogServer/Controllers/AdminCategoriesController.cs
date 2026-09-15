using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PersonalBlogServer.Contracts.Admin;
using PersonalBlogServer.Services.Admin;

namespace PersonalBlogServer.Controllers;

[ApiController]
[Route("api/admin/categories")]
[Authorize]
public class AdminCategoriesController : ControllerBase
{
    private readonly IAdminCategoryService _categoryService;

    public AdminCategoriesController(IAdminCategoryService categoryService)
    {
        _categoryService = categoryService;
    }

    [HttpGet]
    public async Task<IActionResult> GetCategories(
        [FromQuery] string? search,
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 10,
        CancellationToken cancellationToken = default)
    {
        var result = await _categoryService.GetCategoriesAsync(search, page, pageSize, cancellationToken);
        return Ok(result);
    }

    [HttpGet("stats")]
    public async Task<IActionResult> GetStats(CancellationToken cancellationToken = default)
    {
        var stats = await _categoryService.GetStatsAsync(cancellationToken);
        return Ok(stats);
    }

    [HttpGet("{id:int}")]
    public async Task<IActionResult> GetCategoryById(int id, CancellationToken cancellationToken = default)
    {
        var category = await _categoryService.GetCategoryByIdAsync(id, cancellationToken);
        if (category is null)
        {
            return NotFound(new { message = $"Không tìm thấy danh mục có mã #{id}" });
        }

        return Ok(category);
    }

    [HttpPost]
    public async Task<IActionResult> CreateCategory([FromBody] CreateCategoryRequest request, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(request.Name))
        {
            return BadRequest(new { message = "Tên chủ đề & danh mục không được để trống." });
        }

        var created = await _categoryService.CreateCategoryAsync(request, cancellationToken);
        return CreatedAtAction(nameof(GetCategoryById), new { id = created.Id }, created);
    }

    [HttpPut("{id:int}")]
    public async Task<IActionResult> UpdateCategory(int id, [FromBody] UpdateCategoryRequest request, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(request.Name))
        {
            return BadRequest(new { message = "Tên chủ đề & danh mục không được để trống." });
        }

        var updated = await _categoryService.UpdateCategoryAsync(id, request, cancellationToken);
        if (updated is null)
        {
            return NotFound(new { message = $"Không tìm thấy danh mục có mã #{id} để cập nhật." });
        }

        return Ok(updated);
    }

    [HttpDelete("{id:int}")]
    public async Task<IActionResult> DeleteCategory(int id, CancellationToken cancellationToken = default)
    {
        var (success, errorMessage) = await _categoryService.DeleteCategoryAsync(id, cancellationToken);
        if (!success)
        {
            return BadRequest(new { message = errorMessage });
        }

        return Ok(new { message = "Đã xóa chủ đề & danh mục thành công." });
    }
}
