using SpendSmart.Notification.API.Domain.Entities;

namespace SpendSmart.Notification.API.Infrastructure.Repositories;

public interface INotificationRecordRepository
{
    Task<NotificationRecord?> GetByIdAsync(int id);
    Task<List<NotificationRecord>> GetByUserIdAsync(int userId, int page, int pageSize);
    Task<int> GetUnreadCountAsync(int userId);
    Task AddAsync(NotificationRecord notification);
    Task AddRangeAsync(IEnumerable<NotificationRecord> notifications);
    Task MarkAsReadAsync(int notificationRecordId);
    Task MarkAllReadAsync(int userId);
    Task SaveChangesAsync();
}