namespace SpendSmart.Budget.API.Infrastructure.HttpClients;

public interface INotificationHttpClient
{
    Task SendBudgetAlertAsync(int userId, string alertType, decimal percent, int budgetPlanId);
}