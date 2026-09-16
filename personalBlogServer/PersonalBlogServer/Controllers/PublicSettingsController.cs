using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PersonalBlogServer.Services.Admin;

namespace PersonalBlogServer.Controllers;

[ApiController]
[Route("api/public/settings")]
[AllowAnonymous]
public class PublicSettingsController : ControllerBase
{
    private readonly ISiteSettingsService _settingsService;

    public PublicSettingsController(ISiteSettingsService settingsService)
    {
        _settingsService = settingsService;
    }

    [HttpGet]
    public async Task<IActionResult> GetPublicSettings(CancellationToken cancellationToken = default)
    {
        var settings = await _settingsService.GetSettingsAsync(cancellationToken);
        
        // Return only safe public info
        return Ok(new
        {
            general = settings.General,
            seo = settings.Seo,
            reading = settings.Reading,
            comments = new
            {
                enableComments = settings.Comments.EnableComments,
                allowGuestComments = settings.Comments.AllowGuestComments
            },
            contact = settings.Contact,
            maintenance = new
            {
                maintenanceMode = settings.Maintenance.MaintenanceMode,
                maintenanceNotice = settings.Maintenance.MaintenanceNotice
            }
        });
    }
}
