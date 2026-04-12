using System.ComponentModel.DataAnnotations;

namespace SpendSmart.Report.API.Application.DTOs;

public class GenerateReportDto
{
    [Required]
    public int UserId { get; set; }

    [RegularExpression(@"^(MONTHLY|CUSTOM)$", ErrorMessage = "ReportType must be MONTHLY or CUSTOM")]
    public string ReportType { get; set; } = "MONTHLY";

    public int? Month { get; set; }
    public int? Year { get; set; }
    public DateTime? StartDate { get; set; }
    public DateTime? EndDate { get; set; }
}