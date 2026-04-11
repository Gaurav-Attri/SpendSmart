using System.ComponentModel.DataAnnotations;

namespace SpendSmart.Notification.API.Application.DTOs;

public class CreateNotificationRecordDto
{
    [Required]
    public int UserId { get; set; }

    [Required, StringLength(50)]
    [RegularExpression(@"^(BUDGET_WARNING|BUDGET_EXCEEDED|PLATFORM|MONTHLY_SUMMARY|RECURRING_REMINDER)$",
        ErrorMessage = "Type must be BUDGET_WARNING, BUDGET_EXCEEDED, PLATFORM, MONTHLY_SUMMARY, or RECURRING_REMINDER")]
    public string Type { get; set; } = "PLATFORM";

    [Required, StringLength(200)]
    public string Title { get; set; } = string.Empty;

    [Required, StringLength(500)]
    public string Message { get; set; } = string.Empty;

    public int? RelatedId { get; set; }
}