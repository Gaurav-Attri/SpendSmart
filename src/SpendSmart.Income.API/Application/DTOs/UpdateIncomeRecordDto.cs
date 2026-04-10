using System.ComponentModel.DataAnnotations;

namespace SpendSmart.Income.API.Application.DTOs;

public class UpdateIncomeRecordDto
{
    [StringLength(50)]
    [RegularExpression(@"^(SALARY|FREELANCE|INVESTMENT|RENTAL|OTHER)$")]
    public string? Source { get; set; }

    [Range(0.01, 9999999.99)]
    public decimal? Amount { get; set; }

    [StringLength(3)]
    public string? Currency { get; set; }

    [StringLength(500)]
    public string? Description { get; set; }

    public DateTime? Date { get; set; }

    public bool? IsRecurring { get; set; }

    [RegularExpression(@"^(MONTHLY|WEEKLY|YEARLY)$")]
    public string? RecurrenceType { get; set; }
}