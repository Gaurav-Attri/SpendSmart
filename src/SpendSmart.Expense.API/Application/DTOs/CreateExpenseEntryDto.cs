using System.ComponentModel.DataAnnotations;

namespace SpendSmart.Expense.API.Application.DTOs;

public class CreateExpenseEntryDto
{
    [Required]
    public int UserId { get; set; }

    [Required]
    public int CategoryId { get; set; }

    [Required, Range(0.01, 9999999.99)]
    public decimal Amount { get; set; }

    [StringLength(3, MinimumLength = 3)]
    [RegularExpression(@"^[A-Z]{3}$", ErrorMessage = "Currency must be a 3-letter ISO code")]
    public string Currency { get; set; } = "INR";

    [StringLength(500)]
    public string Description { get; set; } = string.Empty;

    public DateTime Date { get; set; } = DateTime.UtcNow;

    [RegularExpression(@"^(CASH|CARD|UPI|NET_BANKING|WALLET)$", 
        ErrorMessage = "PaymentMode must be CASH, CARD, UPI, NET_BANKING, or WALLET")]
    public string PaymentMode { get; set; } = "CASH";

    public string? Tags { get; set; }

    public bool IsRecurring { get; set; } = false;
}