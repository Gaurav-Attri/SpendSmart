namespace SpendSmart.Notification.API.Application.DTOs;

public class NotificationRecordResponseDto
{
    public int NotificationRecordId { get; set; }
    public int UserId { get; set; }
    public string Type { get; set; } = string.Empty;
    public string Title { get; set; } = string.Empty;
    public string Message { get; set; } = string.Empty;
    public int? RelatedId { get; set; }
    public bool IsRead { get; set; }
    public DateTime SentAt { get; set; }
}