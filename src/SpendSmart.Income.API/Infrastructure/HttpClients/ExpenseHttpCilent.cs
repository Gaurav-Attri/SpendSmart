namespace SpendSmart.Income.API.Infrastructure.HttpClients;

public class ExpenseHttpClient : IExpenseHttpClient
{
    private readonly HttpClient _http;
    private readonly ILogger<ExpenseHttpClient> _logger;

    public ExpenseHttpClient(HttpClient http, ILogger<ExpenseHttpClient> logger)
    {
        _http = http;
        _logger = logger;
    }

    public async Task<decimal> GetTotalByUserAsync(int userId)
    {
        try
        {
            var response = await _http.GetAsync($"api/expenses/total/{userId}");
            response.EnsureSuccessStatusCode();
            return await response.Content.ReadFromJsonAsync<decimal>();
        }
        catch (HttpRequestException ex)
        {
            _logger.LogError(ex, "Failed to get expense total for user {UserId}", userId);
            return 0;
        }
    }
}