using System.ComponentModel.DataAnnotations;

namespace SpendSmart.Notification.API.Application.DTOs;

public class BudgetAlertDto
{
    [Required]
    public int UserId { get; set; }

    [Required]
    [RegularExpression(@"^(BUDGET_WARNING|BUDGET_EXCEEDED)$")]
    public string AlertType { get; set; } = string.Empty;

    [Required, Range(0, 1000)]
    public decimal Percent { get; set; }

    [Required]
    public int BudgetId { get; set; }
}