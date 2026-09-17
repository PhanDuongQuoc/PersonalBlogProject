using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PersonalBlogServer.Contracts.Admin;
using PersonalBlogServer.Services.Admin;

namespace PersonalBlogServer.Controllers;

[ApiController]
[Route("api/admin/contacts")]
[Authorize]
public class AdminContactController : ControllerBase
{
    private readonly IAdminContactService _contactService;

    public AdminContactController(IAdminContactService contactService)
    {
        _contactService = contactService;
    }

    [HttpGet]
    public async Task<IActionResult> GetContacts([FromQuery] ContactListQuery query, CancellationToken cancellationToken = default)
    {
        var result = await _contactService.GetContactsAsync(query, cancellationToken);
        return Ok(result);
    }

    [HttpGet("summary")]
    public async Task<IActionResult> GetSummary(CancellationToken cancellationToken = default)
    {
        var summary = await _contactService.GetContactSummaryAsync(cancellationToken);
        return Ok(summary);
    }

    [HttpGet("{id:int}")]
    public async Task<IActionResult> GetContactById(int id, [FromQuery] bool autoMarkAsRead = true, CancellationToken cancellationToken = default)
    {
        var contact = await _contactService.GetContactByIdAsync(id, autoMarkAsRead, cancellationToken);
        if (contact == null)
        {
            return NotFound(new { message = "Không tìm thấy tin nhắn liên hệ." });
        }
        return Ok(contact);
    }

    [HttpPatch("{id:int}/status")]
    public async Task<IActionResult> UpdateStatus(int id, [FromBody] UpdateContactStatusRequest request, CancellationToken cancellationToken = default)
    {
        if (request == null || string.IsNullOrWhiteSpace(request.Status))
        {
            return BadRequest(new { message = "Trạng thái không hợp lệ." });
        }

        var updated = await _contactService.UpdateContactStatusAsync(id, request, cancellationToken);
        if (updated == null)
        {
            return NotFound(new { message = "Không tìm thấy tin nhắn liên hệ để cập nhật." });
        }

        return Ok(new
        {
            success = true,
            message = "Cập nhật trạng thái tin nhắn thành công!",
            data = updated
        });
    }

    [HttpDelete("{id:int}")]
    public async Task<IActionResult> DeleteContact(int id, CancellationToken cancellationToken = default)
    {
        var success = await _contactService.DeleteContactAsync(id, cancellationToken);
        if (!success)
        {
            return NotFound(new { message = "Không tìm thấy tin nhắn liên hệ để xóa." });
        }

        return Ok(new
        {
            success = true,
            message = "Đã xóa tin nhắn liên hệ thành công!"
        });
    }
}
