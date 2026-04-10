namespace SpendSmart.Income.API.Domain.Entities;

public class IncomeRecord
{
    public int IncomeRecordId { get; set; }
    public int UserId { get; set; }
    public string Source { get; set; } = "OTHER";
    public decimal Amount { get; set; }
    public string Currency { get; set; } = "INR";
    public string? Description { get; set; }
    public DateTime Date { get; set; } = DateTime.UtcNow;
    public bool IsRecurring { get; set; } = false;
    public string? RecurrenceType { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? UpdatedAt { get; set; }
}