using Microsoft.EntityFrameworkCore;
using SpendSmart.Notification.API.Domain.Entities;

namespace SpendSmart.Notification.API.Infrastructure.Data;

public class NotificationDbContext : DbContext
{
    public NotificationDbContext(DbContextOptions<NotificationDbContext> options) : base(options) { }

    public DbSet<NotificationRecord> NotificationRecords => Set<NotificationRecord>();

    protected override void OnModelCreating(ModelBuilder mb)
    {
        mb.Entity<NotificationRecord>(e =>
        {
            e.HasKey(n => n.NotificationRecordId);
            e.HasIndex(n => new { n.UserId, n.IsRead });
            e.HasIndex(n => n.SentAt);
            e.HasIndex(n => n.Type);
            e.Property(n => n.Type).HasMaxLength(50);
            e.Property(n => n.Title).HasMaxLength(200);
            e.Property(n => n.Message).HasMaxLength(500);
        });
    }
}