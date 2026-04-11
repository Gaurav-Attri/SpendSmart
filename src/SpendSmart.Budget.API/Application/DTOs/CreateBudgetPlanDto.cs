using System.ComponentModel.DataAnnotations;

namespace SpendSmart.Budget.API.Application.DTOs;

public class CreateBudgetPlanDto
{
    [Required]
    public int UserId { get; set; }

    public int? CategoryId { get; set; }

    [Required, StringLength(100, MinimumLength = 3)]
    public string Name { get; set; } = string.Empty;

    [Required, Range(0.01, 9999999.99)]
    public decimal LimitAmount { get; set; }

    [StringLength(3, MinimumLength = 3)]
    [RegularExpression(@"^[A-Z]{3}$", ErrorMessage = "Currency must be a 3-letter ISO code")]
    public string Currency { get; set; } = "INR";

    [RegularExpression(@"^(MONTHLY|WEEKLY|YEARLY|CUSTOM)$", 
        ErrorMessage = "Period must be MONTHLY, WEEKLY, YEARLY, or CUSTOM")]
    public string Period { get; set; } = "MONTHLY";

    [Required]
    public DateTime StartDate { get; set; }

    [Required]
    public DateTime EndDate { get; set; }
}