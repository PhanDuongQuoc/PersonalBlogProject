using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PersonalBlogServer.Contracts.Public;
using PersonalBlogServer.Services.Public;

namespace PersonalBlogServer.Controllers;

[ApiController]
[Route("api/public/contact")]
[AllowAnonymous]
public class PublicContactController : ControllerBase
{
    private readonly IPublicContactService _contactService;

    public PublicContactController(IPublicContactService contactService)
    {
        _contactService = contactService;
    }

    [HttpPost]
    public async Task<IActionResult> SubmitContact([FromBody] SubmitContactRequest request, CancellationToken cancellationToken = default)
    {
        if (request == null || !ModelState.IsValid)
        {
            return BadRequest(new SubmitContactResponse
            {
                Success = false,
                Message = "Vui lòng kiểm tra lại các trường thông tin bắt buộc."
            });
        }

        var ipAddress = HttpContext.Connection.RemoteIpAddress?.ToString();
        var result = await _contactService.SubmitContactAsync(request, ipAddress, cancellationToken);

        if (!result.Success)
        {
            return StatusCode(500, result);
        }

        return Ok(result);
    }
}
