namespace SpendSmart.Budget.API.Application.DTOs;

public class BudgetPlanResponseDto
{
    public int BudgetPlanId { get; set; }
    public int UserId { get; set; }
    public int? CategoryId { get; set; }
    public string Name { get; set; } = string.Empty;
    public decimal LimitAmount { get; set; }
    public decimal SpentAmount { get; set; }
    public decimal RemainingAmount { get; set; }
    public decimal UtilizationPercentage { get; set; }
    public string Currency { get; set; } = string.Empty;
    public string Period { get; set; } = string.Empty;
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    public bool IsActive { get; set; }
}