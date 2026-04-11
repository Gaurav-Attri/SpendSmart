namespace SpendSmart.Budget.API.Infrastructure.HttpClients;

public class NotificationHttpClient : INotificationHttpClient
{
    private readonly HttpClient _http;
    private readonly ILogger<NotificationHttpClient> _logger;

    public NotificationHttpClient(HttpClient http, ILogger<NotificationHttpClient> logger)
    {
        _http = http;
        _logger = logger;
    }

    public async Task SendBudgetAlertAsync(int userId, string alertType, decimal percent, int budgetPlanId)
    {
        try
        {
            var payload = new { userId, alertType, percent, budgetId = budgetPlanId };
            var response = await _http.PostAsJsonAsync("api/notifications/budget-alert", payload);
            
            if (response.IsSuccessStatusCode)
            {
                _logger.LogInformation("Budget alert sent for user {UserId}, type {AlertType}", userId, alertType);
            }
            else
            {
                _logger.LogWarning("Failed to send budget alert. Status: {StatusCode}", response.StatusCode);
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error sending budget alert for user {UserId}", userId);
        }
    }
}