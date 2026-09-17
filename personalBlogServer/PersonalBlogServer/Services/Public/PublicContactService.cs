using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Logging;
using PersonalBlogServer.Contracts.Admin;
using PersonalBlogServer.Contracts.Public;
using PersonalBlogServer.Hubs;
using PersonalBlogServer.Models;

namespace PersonalBlogServer.Services.Public;

public class PublicContactService : IPublicContactService
{
    private readonly AppDbContext _dbContext;
    private readonly IHubContext<NotificationHub> _hubContext;
    private readonly ILogger<PublicContactService> _logger;

    public PublicContactService(
        AppDbContext dbContext,
        IHubContext<NotificationHub> hubContext,
        ILogger<PublicContactService> logger)
    {
        _dbContext = dbContext;
        _hubContext = hubContext;
        _logger = logger;
    }

    public async Task<SubmitContactResponse> SubmitContactAsync(SubmitContactRequest request, string? ipAddress, CancellationToken cancellationToken = default)
    {
        // 1. Anti-bot honeypot check
        if (request.Botcheck)
        {
            _logger.LogWarning("Spam bot detected via honeypot field from IP: {IpAddress}", ipAddress);
            return new SubmitContactResponse { Success = true, Message = "Tin nhắn của bạn đã được gửi thành công." };
        }

        try
        {
            // 2. Create and save entity
            var contact = new ContactMessage
            {
                Name = request.Name.Trim(),
                Email = request.Email.Trim().ToLower(),
                Subject = !string.IsNullOrWhiteSpace(request.Subject) ? request.Subject.Trim() : "Tin nhắn liên hệ mới từ Portfolio",
                Message = request.Message.Trim(),
                Status = "Unread",
                IpAddress = ipAddress,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };

            _dbContext.ContactMessages.Add(contact);
            await _dbContext.SaveChangesAsync(cancellationToken);

            _logger.LogInformation("New contact message saved with ID: {Id} from {Email}", contact.Id, contact.Email);

            // 3. Broadcast Real-time event via SignalR Hub
            var dto = new ContactMessageDto
            {
                Id = contact.Id,
                Name = contact.Name,
                Email = contact.Email,
                Subject = contact.Subject,
                Message = contact.Message,
                Status = contact.Status,
                IpAddress = contact.IpAddress,
                CreatedAt = contact.CreatedAt.HasValue ? DateTime.SpecifyKind(contact.CreatedAt.Value, DateTimeKind.Utc) : DateTime.UtcNow,
                UpdatedAt = contact.UpdatedAt.HasValue ? DateTime.SpecifyKind(contact.UpdatedAt.Value, DateTimeKind.Utc) : DateTime.UtcNow
            };

            try
            {
                await _hubContext.Clients.All.SendAsync("ReceiveNewContactMessage", dto, cancellationToken);
                _logger.LogInformation("Broadcasted ReceiveNewContactMessage event via SignalR for contact ID: {Id}", contact.Id);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to broadcast SignalR event for contact ID: {Id}", contact.Id);
            }

            return new SubmitContactResponse
            {
                Success = true,
                Message = "Cảm ơn bạn! Tin nhắn của bạn đã được gửi thành công và sẽ được phản hồi sớm nhất.",
                ContactId = contact.Id
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to submit contact message from {Email}", request.Email);
            return new SubmitContactResponse
            {
                Success = false,
                Message = "Không thể gửi tin nhắn lúc này. Vui lòng thử lại sau!"
            };
        }
    }
}
