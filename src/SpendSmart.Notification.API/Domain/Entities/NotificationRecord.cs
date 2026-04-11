namespace SpendSmart.Notification.API.Domain.Entities;

public class NotificationRecord
{
    public int NotificationRecordId { get; set; }
    public int UserId { get; set; }
    public string Type { get; set; } = "PLATFORM";
    public string Title { get; set; } = string.Empty;
    public string Message { get; set; } = string.Empty;
    public int? RelatedId { get; set; }
    public bool IsRead { get; set; } = false;
    public DateTime SentAt { get; set; } = DateTime.UtcNow;
}