using SpendSmart.Report.API.Application.DTOs;

namespace SpendSmart.Report.API.Infrastructure.HttpClients;

public interface IIncomeHttpClient
{
    Task<List<IncomeDto>> GetByUserAsync(int userId, string token);
    Task<decimal> GetTotalByUserAsync(int userId, string token);
}