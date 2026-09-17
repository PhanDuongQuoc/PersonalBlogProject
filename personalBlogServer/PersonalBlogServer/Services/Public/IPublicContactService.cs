using System.Threading;
using System.Threading.Tasks;
using PersonalBlogServer.Contracts.Public;

namespace PersonalBlogServer.Services.Public;

public interface IPublicContactService
{
    Task<SubmitContactResponse> SubmitContactAsync(SubmitContactRequest request, string? ipAddress, CancellationToken cancellationToken = default);
}
