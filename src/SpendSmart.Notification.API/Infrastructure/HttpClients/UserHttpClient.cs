using SpendSmart.Notification.API.Application.DTOs;

namespace SpendSmart.Notification.API.Infrastructure.HttpClients;

public class UserHttpClient : IUserHttpClient
{
    private readonly HttpClient _http;
    private readonly ILogger<UserHttpClient> _logger;

    public UserHttpClient(HttpClient http, ILogger<UserHttpClient> logger)
    {
        _http = http;
        _logger = logger;
    }

    public async Task<List<int>> GetAllActiveUserIdsAsync()
    {
        try
        {
            var response = await _http.GetAsync("api/admin/users/ids");
            response.EnsureSuccessStatusCode();
            return await response.Content.ReadFromJsonAsync<List<int>>() ?? new List<int>();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to get active user IDs from Auth.API");
            return new List<int>();
        }
    }

    public async Task<string?> GetUserEmailAsync(int userId)
    {
        try
        {
            var response = await _http.GetAsync($"api/auth/profile/{userId}");
            if (response.IsSuccessStatusCode)
            {
                var user = await response.Content.ReadFromJsonAsync<UserDto>();
                return user?.Email;
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to fetch user email for {UserId}", userId);
        }
        return null;
    }
}