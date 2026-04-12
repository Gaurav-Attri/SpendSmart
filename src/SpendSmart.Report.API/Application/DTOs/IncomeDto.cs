namespace SpendSmart.Report.API.Application.DTOs;

public class IncomeDto
{
    public int IncomeRecordId { get; set; }
    public int UserId { get; set; }
    public string Source { get; set; } = string.Empty;
    public decimal Amount { get; set; }
    public string Currency { get; set; } = string.Empty;
    public string? Description { get; set; }
    public DateTime Date { get; set; }
}