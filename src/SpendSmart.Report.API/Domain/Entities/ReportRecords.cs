namespace SpendSmart.Report.API.Domain.Entities;

public class ReportRecord
{
    public int ReportRecordId { get; set; }
    public int UserId { get; set; }
    public string ReportType { get; set; } = "MONTHLY";
    public string Title { get; set; } = string.Empty;
    public DateTime GeneratedAt { get; set; } = DateTime.UtcNow;
    public string? FilePath { get; set; }
    public string? Parameters { get; set; }
    public string Status { get; set; } = "GENERATED";
}