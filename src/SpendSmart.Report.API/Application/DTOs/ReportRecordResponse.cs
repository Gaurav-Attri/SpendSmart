namespace SpendSmart.Report.API.Application.DTOs;

public class ReportRecordResponseDto
{
    public int ReportRecordId { get; set; }
    public int UserId { get; set; }
    public string ReportType { get; set; } = string.Empty;
    public string Title { get; set; } = string.Empty;
    public DateTime GeneratedAt { get; set; }
    public string? FilePath { get; set; }
    public string Status { get; set; } = string.Empty;
}