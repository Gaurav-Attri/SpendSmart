namespace SpendSmart.Auth.API.Infrastructure.HttpClients;

public interface ICategoryHttpClient
{
    Task SeedDefaultCategoriesAsync(int userId);
}