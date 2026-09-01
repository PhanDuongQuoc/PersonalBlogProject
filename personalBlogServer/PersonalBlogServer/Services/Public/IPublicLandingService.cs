using PersonalBlogServer.Contracts.Public;

namespace PersonalBlogServer.Services.Public;

public interface IPublicLandingService
{
    Task<PublicLandingResponse?> GetLandingAsync(string? username, CancellationToken cancellationToken = default);
}
