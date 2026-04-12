using System.Net.Http.Headers;
using SpendSmart.Report.API.Application.DTOs;

namespace SpendSmart.Report.API.Infrastructure.HttpClients;

public class ExpenseHttpClient : IExpenseHttpClient
{
    private readonly HttpClient _http;
    private readonly ILogger<ExpenseHttpClient> _logger;

    public ExpenseHttpClient(HttpClient http, ILogger<ExpenseHttpClient> logger)
    {
        _http = http;
        _logger = logger;
    }

    private void SetAuthorizationHeader(string token)
    {
        if (!string.IsNullOrEmpty(token))
        {
            _http.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
        }
    }

    public async Task<List<ExpenseDto>> GetByUserAsync(int userId, string token)
    {
        try
        {
            SetAuthorizationHeader(token);
            var response = await _http.GetAsync($"api/expenses/user/{userId}");
            response.EnsureSuccessStatusCode();
            return await response.Content.ReadFromJsonAsync<List<ExpenseDto>>() ?? new List<ExpenseDto>();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to get expenses for user {UserId}", userId);
            return new List<ExpenseDto>();
        }
    }

    public async Task<List<ExpenseDto>> GetByDateRangeAsync(int userId, DateTime start, DateTime end, string token)
    {
        try
        {
            SetAuthorizationHeader(token);
            var response = await _http.GetAsync($"api/expenses/filter?userId={userId}&start={start:O}&end={end:O}");
            response.EnsureSuccessStatusCode();
            return await response.Content.ReadFromJsonAsync<List<ExpenseDto>>() ?? new List<ExpenseDto>();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to get expenses by date range for user {UserId}", userId);
            return new List<ExpenseDto>();
        }
    }

    public async Task<decimal> GetTotalByUserAsync(int userId, string token)
    {
        try
        {
            SetAuthorizationHeader(token);
            var response = await _http.GetAsync($"api/expenses/total/{userId}");
            response.EnsureSuccessStatusCode();
            return await response.Content.ReadFromJsonAsync<decimal>();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to get expense total for user {UserId}", userId);
            return 0;
        }
    }
}