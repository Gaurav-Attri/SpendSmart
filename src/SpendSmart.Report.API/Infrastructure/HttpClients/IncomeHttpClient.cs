using System.Net.Http.Headers;
using SpendSmart.Report.API.Application.DTOs;

namespace SpendSmart.Report.API.Infrastructure.HttpClients;

public class IncomeHttpClient : IIncomeHttpClient
{
    private readonly HttpClient _http;
    private readonly ILogger<IncomeHttpClient> _logger;

    public IncomeHttpClient(HttpClient http, ILogger<IncomeHttpClient> logger)
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

    public async Task<List<IncomeDto>> GetByUserAsync(int userId, string token)
    {
        try
        {
            SetAuthorizationHeader(token);
            var response = await _http.GetAsync($"api/incomes/user/{userId}");
            response.EnsureSuccessStatusCode();
            return await response.Content.ReadFromJsonAsync<List<IncomeDto>>() ?? new List<IncomeDto>();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to get incomes for user {UserId}", userId);
            return new List<IncomeDto>();
        }
    }

    public async Task<decimal> GetTotalByUserAsync(int userId, string token)
    {
        try
        {
            SetAuthorizationHeader(token);
            var response = await _http.GetAsync($"api/incomes/total/{userId}");
            response.EnsureSuccessStatusCode();
            return await response.Content.ReadFromJsonAsync<decimal>();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to get income total for user {UserId}", userId);
            return 0;
        }
    }
}