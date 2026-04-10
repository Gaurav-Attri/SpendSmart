using System.Net.Http.Headers;

namespace SpendSmart.Auth.API.Infrastructure.HttpClients;

public class CategoryHttpClient : ICategoryHttpClient
{
    private readonly HttpClient _http;
    private readonly ILogger<CategoryHttpClient> _logger;

    public CategoryHttpClient(HttpClient http, ILogger<CategoryHttpClient> logger)
    {
        _http = http;
        _logger = logger;
    }

    public async Task SeedDefaultCategoriesAsync(int userId)
    {
        try
        {
            // You need to get the current JWT token from somewhere
            // For now, Category.API needs to allow anonymous for seeding
            var response = await _http.PostAsync($"api/categories/seed/{userId}", null);
            if (response.IsSuccessStatusCode)
            {
                _logger.LogInformation("Successfully seeded default categories for user {UserId}", userId);
            }
            else
            {
                _logger.LogWarning("Failed to seed categories for user {UserId}. Status: {StatusCode}", userId, response.StatusCode);
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error seeding default categories for user {UserId}", userId);
        }
    }
}