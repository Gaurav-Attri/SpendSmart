using Microsoft.EntityFrameworkCore;
using SpendSmart.Notification.API.Domain.Entities;
using SpendSmart.Notification.API.Infrastructure.Data;

namespace SpendSmart.Notification.API.Infrastructure.Repositories;

public class NotificationRecordRepository : INotificationRecordRepository
{
    private readonly NotificationDbContext _ctx;

    public NotificationRecordRepository(NotificationDbContext ctx)
    {
        _ctx = ctx;
    }

    public Task<NotificationRecord?> GetByIdAsync(int id) =>
        _ctx.NotificationRecords.FindAsync(id).AsTask();

    public Task<List<NotificationRecord>> GetByUserIdAsync(int userId, int page, int pageSize) =>
        _ctx.NotificationRecords
            .Where(n => n.UserId == userId)
            .OrderByDescending(n => n.SentAt)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();

    public Task<int> GetUnreadCountAsync(int userId) =>
        _ctx.NotificationRecords
            .CountAsync(n => n.UserId == userId && !n.IsRead);

    public async Task AddAsync(NotificationRecord notification) =>
        await _ctx.NotificationRecords.AddAsync(notification);

    public async Task AddRangeAsync(IEnumerable<NotificationRecord> notifications) =>
        await _ctx.NotificationRecords.AddRangeAsync(notifications);

    public async Task MarkAsReadAsync(int notificationRecordId)
    {
        var notification = await _ctx.NotificationRecords.FindAsync(notificationRecordId);
        if (notification != null)
        {
            notification.IsRead = true;
            await _ctx.SaveChangesAsync();
        }
    }

    public Task MarkAllReadAsync(int userId) =>
        _ctx.NotificationRecords
            .Where(n => n.UserId == userId && !n.IsRead)
            .ExecuteUpdateAsync(s => s.SetProperty(n => n.IsRead, true));

    public Task SaveChangesAsync() =>
        _ctx.SaveChangesAsync();
}