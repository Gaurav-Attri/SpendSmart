namespace SpendSmart.Report.API.Application.DTOs;

public class MonthlySummaryDto
{
    public decimal TotalExpense { get; set; }
    public decimal TotalIncome { get; set; }
    public decimal NetBalance { get; set; }
    public decimal SavingsRate { get; set; }
    public string TopCategory { get; set; } = string.Empty;
    public Dictionary<string, decimal> CategoryBreakdown { get; set; } = new();
}