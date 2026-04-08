namespace SpendSmart.Auth.Domain.Entities;

public class AuditLog
{
    public int AuditLogId { get; set; }
    public int ActorUserId { get; set; }
    public string Action { get; set; } = string.Empty;
    public string EntityName { get; set; } = string.Empty;
    public string? BeforeValue { get; set; }
    public string? AfterValue { get; set; }
    public DateTime Timestamp { get; set; } = DateTime.UtcNow;
}