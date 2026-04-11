using MailKit.Net.Smtp;
using MailKit.Security;
using MimeKit;
using SpendSmart.Notification.API.Application.DTOs;
using SpendSmart.Notification.API.Domain.Entities;
using SpendSmart.Notification.API.Infrastructure.HttpClients;
using SpendSmart.Notification.API.Infrastructure.Repositories;

namespace SpendSmart.Notification.API.Application.Services;

public class NotificationRecordService : INotificationRecordService
{
    private readonly INotificationRecordRepository _repo;
    private readonly IUserHttpClient _userClient;
    private readonly IConfiguration _config;
    private readonly ILogger<NotificationRecordService> _logger;

    public NotificationRecordService(
        INotificationRecordRepository repo,
        IUserHttpClient userClient,
        IConfiguration config,
        ILogger<NotificationRecordService> logger)
    {
        _repo = repo;
        _userClient = userClient;
        _config = config;
        _logger = logger;
    }

    public async Task<NotificationRecord> CreateNotificationAsync(CreateNotificationRecordDto dto)
    {
        var notification = new NotificationRecord
        {
            UserId = dto.UserId,
            Type = dto.Type,
            Title = dto.Title,
            Message = dto.Message,
            RelatedId = dto.RelatedId,
            SentAt = DateTime.UtcNow,
            IsRead = false
        };

        await _repo.AddAsync(notification);
        await _repo.SaveChangesAsync();
        return notification;
    }

    public async Task SendBudgetAlertAsync(int userId, string alertType, decimal percent, int budgetId)
    {
        var title = alertType == "BUDGET_EXCEEDED" 
            ? "⚠️ Budget Limit Reached!" 
            : "⚠️ Budget Warning (80%)";
        
        var message = alertType == "BUDGET_EXCEEDED" 
            ? $"You have exceeded your budget limit ({percent:F1}% spent). Please review your spending." 
            : $"You have used {percent:F1}% of your budget. You're approaching your limit.";

        var notification = new NotificationRecord
        {
            UserId = userId,
            Type = alertType,
            Title = title,
            Message = message,
            RelatedId = budgetId,
            SentAt = DateTime.UtcNow,
            IsRead = false
        };

        await _repo.AddAsync(notification);
        await _repo.SaveChangesAsync();

        // Send email for critical alerts only
        if (alertType == "BUDGET_EXCEEDED")
        {
            await SendEmailAsync(userId, title, message);
        }
    }

    private async Task SendEmailAsync(int userId, string subject, string body)
    {
        try
        {
            // Fetch user email from Auth.API
            var userEmail = await _userClient.GetUserEmailAsync(userId);
            if (string.IsNullOrEmpty(userEmail))
            {
                _logger.LogWarning("No email found for user {UserId}", userId);
                return;
            }

            var email = new MimeMessage();
            email.From.Add(MailboxAddress.Parse(_config["Email:From"]));
            email.To.Add(MailboxAddress.Parse(userEmail));
            email.Subject = subject;
            email.Body = new TextPart("plain") { Text = body };

            using var smtp = new SmtpClient();
            await smtp.ConnectAsync(
                _config["Email:SmtpHost"]!,
                int.Parse(_config["Email:SmtpPort"]!),
                SecureSocketOptions.StartTls);
            await smtp.AuthenticateAsync(_config["Email:Username"]!, _config["Email:Password"]!);
            await smtp.SendAsync(email);
            await smtp.DisconnectAsync(true);
            
            _logger.LogInformation("Budget alert email sent to {UserEmail}", userEmail);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to send budget alert email for user {UserId}", userId);
        }
    }

    public Task<List<NotificationRecord>> GetUserNotificationsAsync(int userId, int page, int pageSize) =>
        _repo.GetByUserIdAsync(userId, page, pageSize);

    public Task<int> GetUnreadCountAsync(int userId) =>
        _repo.GetUnreadCountAsync(userId);

    public Task MarkAsReadAsync(int notificationId) =>
        _repo.MarkAsReadAsync(notificationId);

    public Task MarkAllReadAsync(int userId) =>
        _repo.MarkAllReadAsync(userId);

    public async Task BroadcastAsync(string title, string message)
    {
        var userIds = await _userClient.GetAllActiveUserIdsAsync();
        
        var notifications = userIds.Select(uid => new NotificationRecord
        {
            UserId = uid,
            Type = "PLATFORM",
            Title = title,
            Message = message,
            SentAt = DateTime.UtcNow,
            IsRead = false
        });

        await _repo.AddRangeAsync(notifications);
        await _repo.SaveChangesAsync();
        
        _logger.LogInformation("Broadcast sent to {Count} users", userIds.Count);
    }
}