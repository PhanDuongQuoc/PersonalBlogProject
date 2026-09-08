using System.Threading;
using System.Threading.Tasks;
using PersonalBlogServer.Contracts.Public;

namespace PersonalBlogServer.Services.Public;

public interface IPublicPostService
{
    Task<PublicPostDetailResponse?> GetPostBySlugAsync(string slug, CancellationToken cancellationToken = default);
    Task<PublicCommentDetailDto?> AddCommentAsync(string slug, CreateCommentRequest request, CancellationToken cancellationToken = default);
}
