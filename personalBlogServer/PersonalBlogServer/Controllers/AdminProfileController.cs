using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PersonalBlogServer.Contracts.Admin;
using PersonalBlogServer.Services.Admin;

namespace PersonalBlogServer.Controllers;

[ApiController]
[Route("api/admin/profile")]
[Authorize]
public class AdminProfileController : ControllerBase
{
    private readonly IAdminProfileService _profileService;

    public AdminProfileController(IAdminProfileService profileService)
    {
        _profileService = profileService;
    }

    private int GetCurrentUserId()
    {
        var idClaim = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (int.TryParse(idClaim, out var userId))
        {
            return userId;
        }
        return 1; // Default author
    }

    // ==========================================
    // 1. Profile Endpoints
    // ==========================================

    [HttpGet]
    public async Task<IActionResult> GetProfile(CancellationToken cancellationToken = default)
    {
        var userId = GetCurrentUserId();
        var profile = await _profileService.GetProfileAsync(userId, cancellationToken);
        if (profile is null)
        {
            return NotFound(new { message = "Không tìm thấy thông tin tác giả." });
        }

        return Ok(profile);
    }

    [HttpPut]
    public async Task<IActionResult> UpdateProfile([FromBody] UpdateAdminProfileRequest request, CancellationToken cancellationToken = default)
    {
        var userId = GetCurrentUserId();
        var updated = await _profileService.UpdateProfileAsync(userId, request, cancellationToken);
        if (updated is null)
        {
            return NotFound(new { message = "Không tìm thấy thông tin tác giả để cập nhật." });
        }

        return Ok(updated);
    }

    [HttpGet("stats")]
    public async Task<IActionResult> GetStats(CancellationToken cancellationToken = default)
    {
        var userId = GetCurrentUserId();
        var stats = await _profileService.GetOverviewStatsAsync(userId, cancellationToken);
        return Ok(stats);
    }

    // ==========================================
    // 2. Skills Endpoints
    // ==========================================

    [HttpGet("skills")]
    public async Task<IActionResult> GetSkills(CancellationToken cancellationToken = default)
    {
        var userId = GetCurrentUserId();
        var skills = await _profileService.GetSkillsAsync(userId, cancellationToken);
        return Ok(skills);
    }

    [HttpPost("skills")]
    public async Task<IActionResult> CreateSkill([FromBody] CreateSkillRequest request, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(request.Name))
        {
            return BadRequest(new { message = "Tên kỹ năng không được để trống." });
        }

        if (string.IsNullOrWhiteSpace(request.Category))
        {
            return BadRequest(new { message = "Nhóm phân loại kỹ năng không được để trống." });
        }

        var userId = GetCurrentUserId();
        var created = await _profileService.CreateSkillAsync(userId, request, cancellationToken);
        return Ok(created);
    }

    [HttpPut("skills/{id:int}")]
    public async Task<IActionResult> UpdateSkill(int id, [FromBody] UpdateSkillRequest request, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(request.Name))
        {
            return BadRequest(new { message = "Tên kỹ năng không được để trống." });
        }

        var userId = GetCurrentUserId();
        var updated = await _profileService.UpdateSkillAsync(userId, id, request, cancellationToken);
        if (updated is null)
        {
            return NotFound(new { message = $"Không tìm thấy kỹ năng #{id} để cập nhật." });
        }

        return Ok(updated);
    }

    [HttpDelete("skills/{id:int}")]
    public async Task<IActionResult> DeleteSkill(int id, CancellationToken cancellationToken = default)
    {
        var userId = GetCurrentUserId();
        var success = await _profileService.DeleteSkillAsync(userId, id, cancellationToken);
        if (!success)
        {
            return NotFound(new { message = $"Không tìm thấy kỹ năng #{id} để xóa." });
        }

        return Ok(new { message = "Đã xóa kỹ năng thành công." });
    }

    // ==========================================
    // 3. Experiences Endpoints
    // ==========================================

    [HttpGet("experiences")]
    public async Task<IActionResult> GetExperiences(CancellationToken cancellationToken = default)
    {
        var userId = GetCurrentUserId();
        var experiences = await _profileService.GetExperiencesAsync(userId, cancellationToken);
        return Ok(experiences);
    }

    [HttpPost("experiences")]
    public async Task<IActionResult> CreateExperience([FromBody] CreateExperienceRequest request, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(request.Role))
        {
            return BadRequest(new { message = "Vị trí / chức danh không được để trống." });
        }

        if (string.IsNullOrWhiteSpace(request.Company))
        {
            return BadRequest(new { message = "Tên công ty / tổ chức không được để trống." });
        }

        var userId = GetCurrentUserId();
        var created = await _profileService.CreateExperienceAsync(userId, request, cancellationToken);
        return Ok(created);
    }

    [HttpPut("experiences/{id:int}")]
    public async Task<IActionResult> UpdateExperience(int id, [FromBody] UpdateExperienceRequest request, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(request.Role))
        {
            return BadRequest(new { message = "Vị trí / chức danh không được để trống." });
        }

        var userId = GetCurrentUserId();
        var updated = await _profileService.UpdateExperienceAsync(userId, id, request, cancellationToken);
        if (updated is null)
        {
            return NotFound(new { message = $"Không tìm thấy kinh nghiệm #{id} để cập nhật." });
        }

        return Ok(updated);
    }

    [HttpDelete("experiences/{id:int}")]
    public async Task<IActionResult> DeleteExperience(int id, CancellationToken cancellationToken = default)
    {
        var userId = GetCurrentUserId();
        var success = await _profileService.DeleteExperienceAsync(userId, id, cancellationToken);
        if (!success)
        {
            return NotFound(new { message = $"Không tìm thấy kinh nghiệm #{id} để xóa." });
        }

        return Ok(new { message = "Đã xóa kinh nghiệm thành công." });
    }

    // ==========================================
    // 4. Educations Endpoints
    // ==========================================

    [HttpGet("educations")]
    public async Task<IActionResult> GetEducations(CancellationToken cancellationToken = default)
    {
        var userId = GetCurrentUserId();
        var educations = await _profileService.GetEducationsAsync(userId, cancellationToken);
        return Ok(educations);
    }

    [HttpPost("educations")]
    public async Task<IActionResult> CreateEducation([FromBody] CreateEducationRequest request, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(request.Institution))
        {
            return BadRequest(new { message = "Tên trường / cơ sở đào tạo không được để trống." });
        }

        if (string.IsNullOrWhiteSpace(request.Degree))
        {
            return BadRequest(new { message = "Bằng cấp / chuyên ngành không được để trống." });
        }

        var userId = GetCurrentUserId();
        var created = await _profileService.CreateEducationAsync(userId, request, cancellationToken);
        return Ok(created);
    }

    [HttpPut("educations/{id:int}")]
    public async Task<IActionResult> UpdateEducation(int id, [FromBody] UpdateEducationRequest request, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(request.Institution))
        {
            return BadRequest(new { message = "Tên trường / cơ sở đào tạo không được để trống." });
        }

        var userId = GetCurrentUserId();
        var updated = await _profileService.UpdateEducationAsync(userId, id, request, cancellationToken);
        if (updated is null)
        {
            return NotFound(new { message = $"Không tìm thấy học vấn #{id} để cập nhật." });
        }

        return Ok(updated);
    }

    [HttpDelete("educations/{id:int}")]
    public async Task<IActionResult> DeleteEducation(int id, CancellationToken cancellationToken = default)
    {
        var userId = GetCurrentUserId();
        var success = await _profileService.DeleteEducationAsync(userId, id, cancellationToken);
        if (!success)
        {
            return NotFound(new { message = $"Không tìm thấy học vấn #{id} để xóa." });
        }

        return Ok(new { message = "Đã xóa học vấn thành công." });
    }
}
