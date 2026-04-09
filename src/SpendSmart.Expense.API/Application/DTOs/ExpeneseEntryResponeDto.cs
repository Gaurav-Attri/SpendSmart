namespace SpendSmart.Expense.API.Application.DTOs;

public class ExpenseEntryResponseDto
{
    public int ExpenseEntryId { get; set; }
    public int UserId { get; set; }
    public int CategoryId { get; set; }
    public decimal Amount { get; set; }
    public string Currency { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public DateTime Date { get; set; }
    public string PaymentMode { get; set; } = string.Empty;
    public string? ReceiptUrl { get; set; }
    public string? Tags { get; set; }
    public bool IsRecurring { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
}