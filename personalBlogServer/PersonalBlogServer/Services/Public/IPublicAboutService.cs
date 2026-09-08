using System.Threading;
using System.Threading.Tasks;
using PersonalBlogServer.Contracts.Public;

namespace PersonalBlogServer.Services.Public;

public interface IPublicAboutService
{
    Task<PublicAboutResponse?> GetAboutAsync(string? username, CancellationToken cancellationToken = default);
}
