using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PersonalBlogServer.Contracts.Admin;
using PersonalBlogServer.Services.Admin;

namespace PersonalBlogServer.Controllers;

[ApiController]
[Route("api/admin/settings")]
[Authorize]
public class AdminSettingsController : ControllerBase
{
    private readonly ISiteSettingsService _settingsService;

    public AdminSettingsController(ISiteSettingsService settingsService)
    {
        _settingsService = settingsService;
    }

    [HttpGet]
    public async Task<IActionResult> GetSettings(CancellationToken cancellationToken = default)
    {
        var settings = await _settingsService.GetSettingsAsync(cancellationToken);
        return Ok(settings);
    }

    [HttpPut]
    public async Task<IActionResult> UpdateSettings([FromBody] SiteSettingsDto request, CancellationToken cancellationToken = default)
    {
        if (request == null)
        {
            return BadRequest(new { message = "Dữ liệu cấu hình không hợp lệ." });
        }

        var updated = await _settingsService.UpdateSettingsAsync(request, cancellationToken);
        return Ok(new
        {
            success = true,
            message = "Cập nhật cấu hình hệ thống thành công!",
            data = updated
        });
    }

    [HttpPost("clear-cache")]
    public async Task<IActionResult> ClearCache(CancellationToken cancellationToken = default)
    {
        var result = await _settingsService.ClearCacheAsync(cancellationToken);
        if (result)
        {
            return Ok(new { success = true, message = "Đã làm mới bộ nhớ đệm (Cache) của hệ thống thành công!" });
        }
        return StatusCode(500, new { success = false, message = "Không thể làm mới bộ nhớ đệm." });
    }
}
