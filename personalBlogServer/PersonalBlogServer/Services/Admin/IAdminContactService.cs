using System.Threading;
using System.Threading.Tasks;
using PersonalBlogServer.Contracts.Admin;

namespace PersonalBlogServer.Services.Admin;

public interface IAdminContactService
{
    Task<ContactListResponse> GetContactsAsync(ContactListQuery query, CancellationToken cancellationToken = default);
    Task<ContactMessageDto?> GetContactByIdAsync(int id, bool autoMarkAsRead = true, CancellationToken cancellationToken = default);
    Task<ContactMessageDto?> UpdateContactStatusAsync(int id, UpdateContactStatusRequest request, CancellationToken cancellationToken = default);
    Task<bool> DeleteContactAsync(int id, CancellationToken cancellationToken = default);
    Task<ContactSummaryDto> GetContactSummaryAsync(CancellationToken cancellationToken = default);
}
