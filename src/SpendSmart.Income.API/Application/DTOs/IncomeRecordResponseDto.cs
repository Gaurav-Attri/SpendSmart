namespace SpendSmart.Income.API.Application.DTOs;

public class IncomeRecordResponseDto
{
    public int IncomeRecordId { get; set; }
    public int UserId { get; set; }
    public string Source { get; set; } = string.Empty;
    public decimal Amount { get; set; }
    public string Currency { get; set; } = string.Empty;
    public string? Description { get; set; }
    public DateTime Date { get; set; }
    public bool IsRecurring { get; set; }
    public string? RecurrenceType { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
}