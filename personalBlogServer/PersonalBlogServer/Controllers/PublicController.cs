using Microsoft.AspNetCore.Mvc;
using PersonalBlogServer.Services.Public;

namespace PersonalBlogServer.Controllers;

[ApiController]
[Route("api/public")]
public sealed class PublicController : ControllerBase
{
    private readonly IPublicLandingService _publicLandingService;

    public PublicController(IPublicLandingService publicLandingService)
    {
        _publicLandingService = publicLandingService;
    }

    [HttpGet("landing")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult> GetLanding([FromQuery] string? username, CancellationToken cancellationToken)
    {
        var landing = await _publicLandingService.GetLandingAsync(username, cancellationToken);
        return landing is null ? NotFound() : Ok(landing);
    }
}
