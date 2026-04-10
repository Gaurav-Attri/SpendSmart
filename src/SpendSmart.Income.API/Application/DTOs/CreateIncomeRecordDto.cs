using System.ComponentModel.DataAnnotations;

namespace SpendSmart.Income.API.Application.DTOs;

public class CreateIncomeRecordDto
{
    [Required]
    public int UserId { get; set; }

    [StringLength(50)]
    [RegularExpression(@"^(SALARY|FREELANCE|INVESTMENT|RENTAL|OTHER)$", 
        ErrorMessage = "Source must be SALARY, FREELANCE, INVESTMENT, RENTAL, or OTHER")]
    public string Source { get; set; } = "OTHER";

    [Required, Range(0.01, 9999999.99)]
    public decimal Amount { get; set; }

    [StringLength(3, MinimumLength = 3)]
    [RegularExpression(@"^[A-Z]{3}$", ErrorMessage = "Currency must be a 3-letter ISO code")]
    public string Currency { get; set; } = "INR";

    [StringLength(500)]
    public string? Description { get; set; }

    public DateTime Date { get; set; } = DateTime.UtcNow;

    public bool IsRecurring { get; set; } = false;

    [RegularExpression(@"^(MONTHLY|WEEKLY|YEARLY)$", 
        ErrorMessage = "RecurrenceType must be MONTHLY, WEEKLY, or YEARLY")]
    public string? RecurrenceType { get; set; }
}