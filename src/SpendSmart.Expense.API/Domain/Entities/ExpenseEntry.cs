namespace SpendSmart.Expense.API.Domain.Entities;

public class ExpenseEntry
{
    public int ExpenseEntryId { get; set; }
    public int UserId { get; set; }
    public int CategoryId { get; set; }
    public decimal Amount { get; set; }
    public string Currency { get; set; } = "INR";
    public string Description { get; set; } = string.Empty;
    public DateTime Date { get; set; } = DateTime.UtcNow;
    public string PaymentMode { get; set; } = "CASH";
    public string? ReceiptUrl { get; set; }
    public string? Tags { get; set; }
    public bool IsRecurring { get; set; } = false;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? UpdatedAt { get; set; }
}