namespace SpendSmart.Budget.API.Domain.Entities;

public class BudgetPlan
{
    public int BudgetPlanId { get; set; }
    public int UserId { get; set; }
    public int? CategoryId { get; set; }
    public string Name { get; set; } = string.Empty;
    public decimal LimitAmount { get; set; }
    public decimal SpentAmount { get; set; } = 0;
    public string Currency { get; set; } = "INR";
    public string Period { get; set; } = "MONTHLY";
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    public bool IsActive { get; set; } = true;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    
    public decimal GetRemainingAmount() => LimitAmount - SpentAmount;
    public decimal GetUtilizationPercentage() => LimitAmount == 0 ? 0 : (SpentAmount / LimitAmount) * 100;
}