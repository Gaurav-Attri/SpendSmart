using System.ComponentModel.DataAnnotations;

namespace SpendSmart.Expense.API.Application.DTOs;

public class UpdateExpenseEntryDto
{
    [Range(0.01, 9999999.99)]
    public decimal? Amount { get; set; }

    [StringLength(500)]
    public string? Description { get; set; }

    public DateTime? Date { get; set; }

    [RegularExpression(@"^(CASH|CARD|UPI|NET_BANKING|WALLET)$")]
    public string? PaymentMode { get; set; }

    public string? Tags { get; set; }

    public bool? IsRecurring { get; set; }
}