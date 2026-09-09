using Microsoft.AspNetCore.Mvc;
using PersonalBlogServer.Contracts.Public;
using PersonalBlogServer.Services.Public;

namespace PersonalBlogServer.Controllers;

[ApiController]
[Route("api/public")]
public sealed class PublicController : ControllerBase
{
    private readonly IPublicLandingService _publicLandingService;
    private readonly IPublicAboutService _publicAboutService;
    private readonly IPublicPostService _publicPostService;
    private readonly IPublicTopicService _publicTopicService;

    public PublicController(
        IPublicLandingService publicLandingService,
        IPublicAboutService publicAboutService,
        IPublicPostService publicPostService,
        IPublicTopicService publicTopicService)
    {
        _publicLandingService = publicLandingService;
        _publicAboutService = publicAboutService;
        _publicPostService = publicPostService;
        _publicTopicService = publicTopicService;
    }

    [HttpGet("landing")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult> GetLanding([FromQuery] string? username, CancellationToken cancellationToken)
    {
        var landing = await _publicLandingService.GetLandingAsync(username, cancellationToken);
        return landing is null ? NotFound() : Ok(landing);
    }

    [HttpGet("about")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult> GetAbout([FromQuery] string? username, CancellationToken cancellationToken)
    {
        var about = await _publicAboutService.GetAboutAsync(username, cancellationToken);
        return about is null ? NotFound() : Ok(about);
    }

    [HttpGet("posts/{slug}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<PublicPostDetailResponse>> GetPostBySlug([FromRoute] string slug, CancellationToken cancellationToken)
    {
        var post = await _publicPostService.GetPostBySlugAsync(slug, cancellationToken);
        return post is null ? NotFound() : Ok(post);
    }

    [HttpPost("posts/{slug}/comments")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<PublicCommentDetailDto>> AddComment(
        [FromRoute] string slug,
        [FromBody] CreateCommentRequest request,
        CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(request.Content))
        {
            return BadRequest("Nội dung bình luận không được để trống.");
        }

        var comment = await _publicPostService.AddCommentAsync(slug, request, cancellationToken);
        return comment is null ? NotFound() : Ok(comment);
    }

    [HttpGet("topics")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<ActionResult<IReadOnlyList<PublicTopicSummaryResponse>>> GetTopics(
        [FromQuery] string? username,
        [FromQuery] string? search,
        CancellationToken cancellationToken)
    {
        var topics = await _publicTopicService.GetTopicsAsync(username, search, cancellationToken);
        return Ok(topics);
    }

    [HttpGet("topics/{slug}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<PublicTopicDetailResponse>> GetTopicBySlug(
        [FromRoute] string slug,
        [FromQuery] string? username,
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 6,
        [FromQuery] string? sortBy = "latest",
        [FromQuery] string? search = null,
        [FromQuery] string? tag = null,
        CancellationToken cancellationToken = default)
    {
        var topicDetail = await _publicTopicService.GetTopicBySlugAsync(
            slug,
            username,
            page,
            pageSize,
            sortBy,
            search,
            tag,
            cancellationToken);

        return topicDetail is null ? NotFound() : Ok(topicDetail);
    }
}
