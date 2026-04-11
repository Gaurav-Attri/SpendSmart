using SpendSmart.Notification.API.Application.DTOs;
using SpendSmart.Notification.API.Domain.Entities;

namespace SpendSmart.Notification.API.Application.Services;

public interface INotificationRecordService
{
    Task<NotificationRecord> CreateNotificationAsync(CreateNotificationRecordDto dto);
    Task SendBudgetAlertAsync(int userId, string alertType, decimal percent, int budgetId);
    Task<List<NotificationRecord>> GetUserNotificationsAsync(int userId, int page, int pageSize);
    Task<int> GetUnreadCountAsync(int userId);
    Task MarkAsReadAsync(int notificationId);
    Task MarkAllReadAsync(int userId);
    Task BroadcastAsync(string title, string message);
}